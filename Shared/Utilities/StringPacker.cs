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

        public static byte[] PackList(List<string> list)
        {
            var listLength = list.Count;

            var packedBytes = new List<byte>();
            packedBytes.AddRange(BitConverter.GetBytes(listLength));

            foreach(var item in list)
            {
                packedBytes.AddRange(PackString(item));
            }

            return packedBytes.ToArray();
        }

        public static List<string> UnpackList(byte[] encodedList, out byte[] remainingBuffer)
        {
            // Lets unpack 'em!
            var length = BitConverter.ToInt32(encodedList.Take(4).ToArray(), 0); // not sure if i need the take 4 but just to be safe

            var result = new List<string>();
            remainingBuffer = encodedList.Skip(4).ToArray();

            for(int i = 0; i < length; i++)
            {
                result.Add(UnpackString(remainingBuffer, out remainingBuffer));
            }

            return result;
        }
    }
}
