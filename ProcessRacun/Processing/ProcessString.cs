using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace ProcessRacun.Processing
{
    public static class ProcessString
    {
        public static JsonObject ParseResponseAsJsonObj(HttpResponseMessage response)
        {
            string resp = response.Content.ReadAsStringAsync().Result;
            JsonObject JO = new JsonObject();
            JO.Add("ukupanIznos", ParseBillAmount(resp));
            JO.Add("racunCeo", ParseBillWOutQRCode(resp));
            JO.Add("base64Value", ParseBase64Value(ParseBill(resp)));
            return JO;
        }

        private static string ParseBillAmount(string response)
        {
            string startDelimiter = "Укупан износ:";
            string endDelimiter = "Готовина";

            int startIndex = response.IndexOf(startDelimiter);
            if (startIndex == -1)
                return null;


            startIndex += startDelimiter.Length;
            int endIndex = response.IndexOf(endDelimiter, startIndex);
            if (endIndex == -1)
                return null;

            string extractedText = response.Substring(startIndex, endIndex - startIndex);

            return extractedText.Trim();
        }

        private static string ParseBill(string response)
        {
            string startDelimiter = "============ ФИСКАЛНИ РАЧУН ============";
            string endDelimiter = "======== КРАЈ ФИСКАЛНОГ РАЧУНА =========";

            int startIndex = response.IndexOf(startDelimiter) - 1;
            if (startIndex == -1)
                return null;


            int endIndex = response.IndexOf(endDelimiter, startIndex) + 1;
            if (endIndex == -1)
                return null;

            endIndex += endDelimiter.Length;
            string extractedText = response.Substring(startIndex, endIndex - startIndex);
            extractedText = extractedText.Replace("======== КРАЈ ФИСКАЛНОГ РАЧУНА =========", "");
            extractedText = extractedText.Replace("<br/>", "");
            extractedText = extractedText.Replace(" <", "");
            return extractedText.Trim();
        }

        private static string ParseBillWOutQRCode(string response)
        {
            string extractedText = ParseBill(response);
            string parsedBase64Value = ParseBase64Value(extractedText);

            extractedText = extractedText.Replace(parsedBase64Value, "");
            extractedText = extractedText.Replace("<img src=data:image/gif;base64,", "");
            extractedText = extractedText.Replace("width='250' height='250'/>\r\n", "");

            return extractedText.Trim();
        }

        private static string ParseBase64Value(string response)
        {
            string startDelimiter = "<img src=data:image/gif;base64,";
            string endDelimiter = "width='250' height='250'/>\r\n";

            int startIndex = response.IndexOf(startDelimiter);
            if (startIndex == -1)
                return null;

            startIndex += startDelimiter.Length;

            int endIndex = response.IndexOf(endDelimiter, startIndex);
            if (endIndex == -1)
                return null;

            string base64String = response.Substring(startIndex, endIndex - startIndex);

            return base64String.Trim();
        }
    }
}
