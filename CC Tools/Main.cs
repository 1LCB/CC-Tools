using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CC_Tools
{
    public partial class Main : Form
    {
        bool CANCEL = false;
        string random_bins_path;
        Random rdm = new Random();
        int CheckBin(int bin)
        {
            var filename = @"\random-bins-results.txt";
            var url = "https://lookup.binlist.net/" + bin;

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36 OPR/93.0.0.0");

            checkingBinsLog.Items.Add($"Checking: {bin}");
            lblcheckedlog.Text = "" + (Convert.ToInt32(lblcheckedlog.Text) + 1);

            var response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {

                var results = response.Content.ReadAsStringAsync().Result;
                var result_bin = JsonConvert.DeserializeObject<Bin>(results);

                var card_scheme = result_bin.Scheme;
                var card_type = result_bin.Type;
                var card_brand = result_bin.Brand;
                var card_prepaid = result_bin.Prepaid;

                var country_name = result_bin.Country != null ? result_bin.Country.Name : "";

                var bank_name = result_bin.Bank != null ? result_bin.Bank.Name : "";
                var bank_url = result_bin.Bank != null ? result_bin.Bank.Url : "";
                var bank_phone = result_bin.Bank != null ? result_bin.Bank.Phone : "";
                var bank_city = result_bin.Bank != null ? result_bin.Bank.City : "";

                var text =
$@"=====>>> {bin} <<<=====

BIN: {bin}

SCHEME: {card_scheme}
TYPE: {card_type}
BRAND: {card_brand}
PREPAID: {card_prepaid}

COUNTRY: {country_name}

BANK NAME: {bank_name}
BANK URL: {bank_url}
BANK PHONE: {bank_phone}
BANK CITY: {bank_city}

=====>>> {bin} <<<=====

";

                if (
                    result_bin.Bank != null &&
                    result_bin.Country != null &&
                    !string.IsNullOrEmpty(result_bin.Brand) &&
                    !string.IsNullOrEmpty(result_bin.Type) &&
                    !string.IsNullOrEmpty(result_bin.Scheme)
                    ) //
                {
                    using (StreamWriter w = File.AppendText(random_bins_path + filename))
                        w.WriteLine(text);

                    validBinsLog.Items.Add(bin);
                    lblvalidlog.Text = "" + (Convert.ToInt32(lblvalidlog.Text) + 1);
                }
            }
            return (int)response.StatusCode;
        }
        public Main()
        {
            InitializeComponent();
        }

      
        private void button1_Click(object sender, EventArgs e)
        {
            txtValidCCs.Clear();
            txtInvalidCC.Clear();

            checkingBar.Value = 0;
            checkingBar.Maximum = txtCheckCCs.Lines.Length;

            var validCards = 0;
            var invalidCards = 0;
            foreach (var line in txtCheckCCs.Lines)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(line) || line == "\n")
                        continue;

                    Regex rg = new Regex(@"\d{12,}"); //simple regex for cc
                    var cc = rg.Matches(line)[0].Value;

                    if (CheckerMethods.ValidCC(cc) && CheckerMethods.ValidCCUsingRegex(cc)) {
                        txtValidCCs.Text += $"{line}\n";
                        validCards++;
                    }
                    else {
                        txtInvalidCC.Text += $"{line}\n";
                        invalidCards++;
                    }

                    checkingBar.Value += 1;
                }
                catch
                {
                    txtInvalidCC.Text += $"{line}\n";
                    Debug.WriteLine($"\"{line}\" invalid!");
                    invalidCards++;
                    continue;
                }
                lblValid.Text = validCards.ToString();
                lblInvalid.Text = invalidCards.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog opf = new OpenFileDialog())
            {
                opf.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                opf.Filter = "Text files | *.txt; *.png";
                if (opf.ShowDialog() == DialogResult.OK)
                {
                    string[] cc = File.ReadAllLines(opf.FileName);
                    txtCheckCCs.Text = string.Join("\n", cc);
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            txtCheckCCs.Clear();
            var a = MessageBox.Show("Do you want to clear these textboxes below?", "Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (a == DialogResult.Yes)
            {
                txtValidCCs.Clear();
                txtInvalidCC.Clear();

                lblInvalid.Text = "0";
                lblValid.Text = "0";
            }
            checkingBar.Value = 0;
                
        }

        private void button5_Click(object sender, EventArgs e) //generate a new card
        {
            Generator.RunWorkerAsync();
            btnGenerateCC.Enabled = false;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            pnlChecker.BringToFront();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pnlBins.SendToBack();
            pnlGenerator.BringToFront();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            txtbin.Clear();
            for (int i = 0; i < 6; i++)
                txtbin.Text += rdm.Next(0, 10);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            txtNewCards.Clear();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtNewCards.Text);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            var month = "" + rdm.Next(1, 13);
            var year = rdm.Next(2024, 2027);
            if (month.Length < 2)
                month = "0" + month;

            txtExpirationDate.Text = $"{month}|{year}";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            txtCVV.Text = "" + rdm.Next(100, 1000);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/1LCB");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pnlBins.BringToFront();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            var bin = txtBinValidate.Text;
            var url = "https://lookup.binlist.net/" + bin;

            if (string.IsNullOrEmpty(bin))
                return;

            HttpClient client = new HttpClient();

            lblstatus.Text = "Status: Searching...";

            var response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                var results = response.Content.ReadAsStringAsync().Result;
                var result_bin = JsonConvert.DeserializeObject<Bin>(results);
                try
                {
                    txtLength.Text = result_bin.Number != null ? result_bin.Number.Length : "";
                    txtLuhn.Text = result_bin.Number != null ? result_bin.Number.Luhn : "";

                    txtScheme.Text = result_bin.Scheme;
                    txtType.Text = result_bin.Type;
                    txtBrand.Text = result_bin.Brand;
                    txtPrepaid.Text = result_bin.Prepaid;

                    txtCountryName.Text = result_bin.Country != null ? result_bin.Country.Name : "";
                    txtCountryCurrency.Text = result_bin.Country != null ? result_bin.Country.Currency : "";
                    txtCountryAlpha2.Text = result_bin.Country != null ? result_bin.Country.Alpha2 : "";

                    txtBankName.Text = result_bin.Bank != null ? result_bin.Bank.Name : "";
                    txtBankUrl.Text = result_bin.Bank != null ? result_bin.Bank.Url : "";
                    txtBankPhone.Text = result_bin.Bank != null ? result_bin.Bank.Phone : "";
                    txtBankCity.Text = result_bin.Bank != null ? result_bin.Bank.City : "";

                    lblstatus.Text = "Status: Found!";
                }
                catch (Exception ex)
                {
                    lblstatus.Text = "Status: Error";
                }
            }
            else
            {
                lblstatus.Text = "Status: " + response.StatusCode.ToString();
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sf = new SaveFileDialog())
            {
                sf.FileName = $"VALID-CCs-{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}";
                sf.DefaultExt = ".txt";
                sf.AddExtension = true;
                sf.Filter = "Text files |*.txt";
                if (sf.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sf.FileName, txtValidCCs.Text);
                    MessageBox.Show($"Your CCs were saved in\n\"{sf.FileName}\"", "Saved!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        
        private void btnRandomBinChecker_Click(object sender, EventArgs e)
        {
            if (!txtrandombincheckerpath.Enabled)
                return;

            if (btnRandomBinChecker.BackColor == Color.Maroon)
            {
                btnRandomBinChecker.Text = "On";
                btnRandomBinChecker.BackColor = Color.Lime;
                backgroundWorker1.RunWorkerAsync();
                return;
            }
            btnRandomBinChecker.Text = "Off";
            btnRandomBinChecker.BackColor = Color.Maroon;
            
        }

        private void button14_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fb = new FolderBrowserDialog())
            {
                fb.ShowNewFolderButton = true;
                if (fb.ShowDialog() == DialogResult.OK)
                {
                    txtrandombincheckerpath.Text = fb.SelectedPath;
                    random_bins_path = fb.SelectedPath;
                    btnRandomBinChecker.Enabled = true;
                    txtrandombincheckerpath.Enabled = true;
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (btnRandomBinChecker.Text == "On")
            {
                var bin = rdm.Next(40000, 7999999);
                var statuscode = CheckBin(bin);
                if (statuscode == 429)
                {
                    Debug.WriteLine("Waiting for 15s...");
                    checkingBinsLog.Items.Add("Waiting...");
                    Thread.Sleep(15 * 1000);
                    CheckBin(bin);
                } 
                Thread.Sleep(3300); 
            }
        }

        private void CHECKER_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Binners b = new Binners();
            b.Show();
        }

        private void Generator_DoWork(object sender, DoWorkEventArgs e)
        {
            CANCEL = false;
            progressBar1.Value = 0;

            var length = (int)nLength.Value;
            var nocards = (int)numericUpDown1.Value;
            progressBar1.Maximum = nocards;

            for (int i = 0; i < nocards; i++)
            {
                if (CANCEL)
                    break;

                var cc = CheckerMethods.RandomCardGenerator(length, txtbin.Text, txtCVV.Text, txtExpirationDate.Text, chkValidCards.Checked, chkValidRegex.Checked, chkED.Checked, chkCVV.Checked);
                if(cc == null)
                {
                    i--;
                    continue;
                }

                txtNewCards.Text += cc;
                progressBar1.Value += 1;

                Thread.Sleep(1);
            }
            btnGenerateCC.Enabled = true;
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            CANCEL = true;
        }
    }
}
