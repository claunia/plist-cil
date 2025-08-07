// <copyright file="ValidatingCompositeStream.cs" company="Quamotion">
// Copyright (c) Quamotion. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace plistcil.test;

/// <summary>
///     A <see cref="Stream" /> which writes its output to a <see cref="Stream" /> and validates that the data which
///     is being written to the output stream matches the data in a reference stream.
/// </summary>
internal class ValidatingStream : Stream
{
    readonly Stream expectedOutput;
    readonly Stream output;

    /// <summary>Initializes a new instance of the <see cref="ValidatingCompositeStream" /> class.</summary>
    /// <param name="output">The <see cref="Stream" /> to which to write data.</param>
    /// <param name="expectedOutput">The reference stream for <paramref name="output" />.</param>
    public ValidatingStream(Stream output, Stream expectedOutput)
    {
        this.output         = output         ?? throw new ArgumentNullException(nameof(output));
        this.expectedOutput = expectedOutput ?? throw new ArgumentNullException(nameof(expectedOutput));
    }

    /// <inheritdoc />
    public override bool CanRead => false;

    /// <inheritdoc />
    public override bool CanSeek => false;

    /// <inheritdoc />
    public override bool CanWrite => true;

    /// <inheritdoc />
    public override long Length => output.Length;

    /// <inheritdoc />
    public override long Position
    {
        get => output.Position;
        set => throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override void Flush() => output.Flush();

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    /// <inheritdoc />
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    /// <inheritdoc />
    public override void SetLength(long value) => throw new NotImplementedException();

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        byte[] expected = new byte[buffer.Length];
        expectedOutput.Read(expected, offset, count);

        byte[] bufferChunk   = buffer.Skip(offset).Take(count).ToArray();
        byte[] expectedChunk = expected.Skip(offset).Take(count).ToArray();

        // Make sure the data being writen matches the data which was written to the expected stream.
        // This will detect any errors as the invalid data is being written out - as opposed to post-
        // test binary validation.
        Assert.Equal(expectedChunk, bufferChunk);
        output.Write(buffer, offset, count);
    }

    /// <inheritdoc />
    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        byte[] expected = new byte[buffer.Length];
        await expectedOutput.ReadAsync(expected, offset, count, cancellationToken).ConfigureAwait(false);

        byte[] bufferChunk   = buffer.Skip(offset).Take(count).ToArray();
        byte[] expectedChunk = expected.Skip(offset).Take(count).ToArray();

        // Make sure the data being writen matches the data which was written to the expected stream.
        // This will detect any errors as the invalid data is being written out - as opposed to post-
        // test binary validation.
        Assert.Equal(expectedChunk, bufferChunk);

        await output.WriteAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
    }
}