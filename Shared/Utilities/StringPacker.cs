using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Utilities
{
    public static class StringPacker
    {
        public static byte[] PackString(string package)
        {
            List<byte> stringBytes = Encoding.Unicode.GetBytes(package).ToList();
            int stringLength = stringBytes.Count;
            var byteLength = BitConverter.GetBytes(stringLength).ToList();

            var joinedLists = new List<byte>();
            joinedLists.AddRange(byteLength);
            joinedLists.AddRange(stringBytes);

            return joinedLists.ToArray();
        }

        public static string UnpackString(byte[] encodedString, out byte[] remainingBuffer)
        {
            byte[] byteLength = encodedString.Take(4).ToArray();

            int stringLength = BitConverter.ToInt32(byteLength, 0);

            byte[] packedString = encodedString.Skip(4).Take(stringLength).ToArray();

            string result = Encoding.Unicode.GetString(packedString);

            remainingBuffer = encodedString.Skip(4 + stringLength).ToArray();

            return result;
        }
    }
}
