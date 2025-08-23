using K4os.Hash.xxHash;
using System.Buffers;
using System.Text;

namespace SRPM_Services.Extensions.Utils;
public class StringUtils
{
    public static string DecodeHtmlEntitiesText(string? rawText)
    {
        if (string.IsNullOrEmpty(rawText)) return "";
        var text = HtmlAgilityPack.HtmlEntity.DeEntitize(rawText); // giải mã HTML entities
        text = text.Replace('\u00A0', ' ');                     // NBSP -> space thường
        return text.Trim();
    }
    public static class HtmlFingerprint
    {
        // sample this many chars from head and tail
        private const int SampleSize = 1024;

        public static ulong ComputeSampleFingerprint(string html)
        {
            if (html.Length <= SampleSize * 2)
                return ComputeXxHash64(html);

            // take head + tail into a temporary span
            Span<char> sample = stackalloc char[SampleSize * 2];
            html.AsSpan(0, SampleSize).CopyTo(sample);
            html.AsSpan(html.Length - SampleSize, SampleSize)
                .CopyTo(sample.Slice(SampleSize));
            return ComputeXxHash64(sample);
        }

        public static ulong ComputeXxHash64(ReadOnlySpan<char> span)
        {
            // 1) Rent a byte buffer from ArrayPool to avoid repeated allocations
            const int ChunkChars = 8192;
            int maxBytes = Encoding.UTF8.GetMaxByteCount(ChunkChars);
            byte[] buffer = ArrayPool<byte>.Shared.Rent(maxBytes);

            try
            {
                // 2) Instantiate the streaming hasher
                var hasher = new XXH64();

                // 3) Optional: Use a stateless Encoder to handle surrogate pairs correctly
                Encoder encoder = Encoding.UTF8.GetEncoder();

                // 4) Process in char‐chunks
                for (int i = 0; i < span.Length; i += ChunkChars)
                {
                    var slice = span.Slice(i, Math.Min(ChunkChars, span.Length - i));
                    bool completed;

                    // Encode chars → bytes (handles partial surrogates)
                    encoder.Convert(
                        chars: slice.ToArray(),
                        charIndex: 0,
                        charCount: slice.Length,
                        bytes: buffer,
                        byteIndex: 0,
                        byteCount: maxBytes,
                        flush: false,
                        out int charsUsed,
                        out int bytesUsed,
                        out completed
                    );

                    // Feed each chunk to the hasher
                    hasher.Update(buffer.AsSpan(0, bytesUsed));
                }

                // 5) Finalize hash
                return hasher.Digest();
            }
            finally
            {
                // 6) Always return the buffer
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }


        // overload for string
        public static ulong ComputeXxHash64(string html) =>
            ComputeXxHash64(html.AsSpan());
    }
}
