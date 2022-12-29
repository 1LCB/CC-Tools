using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CC_Tools
{
    class CheckerMethods
    {
        private static Random rdm = new Random();
        public static bool ValidCC(string CC)
        {
            int nDigits = CC.Length;

            int nSum = 0;
            bool isSecond = false;
            for (int i = nDigits - 1; i >= 0; i--)
            {
                int d = CC[i] - '0';

                if (isSecond == true)
                    d = d * 2;

                nSum += d / 10; //=> 9/10 => 0.9 => >>> 0 <<<
                nSum += d % 10; //=> 0.5 => >>> 5 <<<

                isSecond = !isSecond;
            }
            return (nSum % 10 == 0);
        }
        public static bool ValidCCUsingRegex(string CC)
        {
            Dictionary<string, string> regexes = new Dictionary<string, string>();

            regexes.Add("MasterCard", @"^(5[1-5]\d{4}|2(2(2[1-9]\d{2}|[3-9]\d{3})|[3-6]\d{4}|7([01]\d{3}|20\d{2})))"); //
            regexes.Add("Visa", @"^(4\d{12,})"); //
            regexes.Add("DinersClub", @"(^3[689]\d{4})|(^30[0-59]\d{10,})"); //
            regexes.Add("AmericanExpress", @"^3[47]\d{4}");//
            regexes.Add("Enroute", @"^(2014|2149)");//
            regexes.Add("Voyager", @"^(8699)");
            regexes.Add("JCB", "^(35(2[89]|[3-8][0-9]))|^(2131|1800)|^(3088|3096|3112|3158|3337)|(2100|1800)"); //
            regexes.Add("Discover", @"^((6011)|(65)|(64[4-9])|(6221(2[6-9])|622[3-9]))"); //
            regexes.Add("VisaElectron", @"^(4026|417500|4508|4844|491[37])"); //

            foreach (var value in regexes.Values)
            {
                Regex rg = new Regex(value);
                var matches = rg.Matches(CC);
                if (matches.Count > 0)
                {
                    Debug.WriteLine($"{CC} = {regexes.FirstOrDefault(x => x.Value == value).Key}");
                    return true;
                }
                    
            }
            return false;
        }
        public static string RandomCardGenerator(int length = 16, string bin = "", string cvv = "", string date = "", 
            bool onlyvalid = true,
            bool validregex = true,
            bool addExpirationDate = true,
            bool addCVV = true)
        {
            const int MAX_ATTEMPTS = 10;
            int attempts = 0;

            Random rdm = new Random();

            var expirationdate = date.Length == 7 ? date : RandomDate();
            var cvv_card = cvv.Length == 3 ? cvv : RandomCVV(cvv);

            while (true)
            {
                Thread.Sleep(1);

                if (attempts >= MAX_ATTEMPTS)
                    return null;

                attempts++;

                var rand_card = "";
                var bin_ = bin;

                while (bin_.Length < 6) // 6 digits bin
                    bin_ += rdm.Next(0, 10);

                for (int j = 0; j < length - bin_.Length; j++)
                    rand_card += rdm.Next(0, 10);

                var cc = bin_ + rand_card;
                var final_card = cc + (addExpirationDate ? "|" + expirationdate : "") + (addCVV ? "|" + cvv_card : "") + "\n";
                Debug.Write(final_card);

                if (validregex && onlyvalid && ValidCC(cc) && ValidCCUsingRegex(cc)) //both
                    return final_card;

                else if (!validregex && onlyvalid && ValidCC(cc)) //only luhn
                    return final_card;

                else if (validregex && !onlyvalid && ValidCCUsingRegex(cc)) //only regex
                    return final_card;

                else if (!validregex && !onlyvalid) //no verification
                    return final_card;

            }
        }

        private static string RandomCVV(string cvv)
        {
            var cvv_ = cvv;
            while (cvv_.Length < 3)
                cvv_ += rdm.Next(0, 10);

            return cvv_;
        }

        private static string RandomDate()
        {
            var month = rdm.Next(1, 13).ToString();
            if (month.Length == 1)
                month = "0" + month;

            return $"{month}|{rdm.Next(2023, 2028)}";
        }
    }
}
