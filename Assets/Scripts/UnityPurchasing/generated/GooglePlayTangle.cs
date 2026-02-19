// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("B4SKhbUHhI+HB4SEhVcN0Qc95Da1B4SntYiDjK8DzQNyiISEhICFhjct2D/pfoffMEZvNhujB03vnlzD0lYMeXseWUIw440TvqEKthGpfy7YfqdS9Enp+8rau8SeuzfQxC+cNuwW44YgYlrwzKiD63fx47DnLdLZcNNEZXvdR10sxfPGql5XTVFk2AJW/PzWFPQSnwfIFEYxwLzP7bR1muMEu84gDFQ62PaoYxnFYFNnlakufLlT9JxzCL7d8Fzdf83GLg+oyQxFUKfOTECVS6Lxe2SGS3ksqulIlmFHZ//6vpVbGDKMdFhvPXWYPjiVmCd0WDHG4PEoZ8W5iKOHi88A3xAvfhDiYg7aquuBhsoQEckKdvpl8BXWo+6rwudsvIeGhIWE");
        private static int[] order = new int[] { 1,1,11,13,13,8,8,8,13,13,10,13,12,13,14 };
        private static int key = 133;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
