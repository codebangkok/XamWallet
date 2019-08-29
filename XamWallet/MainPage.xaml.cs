using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Blockcoli.Libra.Net.Client;
using Blockcoli.Libra.Net.Wallet;
using Xamarin.Essentials;

namespace XamWallet
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        LibraWallet wallet;
        LibraClient client;
        Account account;
      

        public MainPage()
        {
            InitializeComponent();
            //createAccountButton.Clicked += CreateAccountButton_Clicked;
            mintButton.Clicked += MintButton_Clicked;
            balanceButton.Clicked += BalanceButton_Clicked;
            transferButton.Clicked += TransferButton_Clicked;

            client = new LibraClient(LibraNetwork.Testnet);
        }

        protected override void OnAppearing()
        {
            var mnemonic = Preferences.Get("Mnemonic", "");
            if (mnemonic == "")
            {
                wallet = new LibraWallet();
                Preferences.Set("Mnemonic", wallet.Mnemonic.ToString());
            }
            else wallet = new LibraWallet(mnemonic);

            account = wallet.NewAccount();
            addressEntry.Text = account.Address;
            mnemonicEntry.Text = wallet.Mnemonic.ToString();
        }

        private async void TransferButton_Clicked(object sender, EventArgs e)
        {
            var amount = ulong.Parse(transferAmountEntry.Text) * 1000000;
            var accepted = await client.TransferCoins(account, receiverEntry.Text, amount);
            await DisplayAlert("Accepted", accepted.ToString(), "OK");
        }

        private async void BalanceButton_Clicked(object sender, EventArgs e)
        {
            var state = await client.QueryBalance(account.Address);
            balanceLabel.Text = $"Balance: {(float)state.Balance/1000000}";
        }

        private async void MintButton_Clicked(object sender, EventArgs e)
        {
            var amount = ulong.Parse(mintAmoutEntry.Text) * 1000000;
            try
            {
                var sequenceNumber = await client.MintWithFaucetService(account.Address, amount);
                await DisplayAlert("Sequence Number", sequenceNumber.ToString(), "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        //private void CreateAccountButton_Clicked(object sender, EventArgs e)
        //{
        //    var mnemonic = Preferences.Get("Mnemonic", "");
        //    if (mnemonic == "")
        //    {
        //        wallet = new LibraWallet();
        //        Preferences.Set("Mnemonic", wallet.Mnemonic.ToString());
        //    }
        //    else wallet = new LibraWallet(mnemonic);
            
        //    account = wallet.NewAccount();
        //    addressEntry.Text = account.Address;
        //    mnemonicEntry.Text = wallet.Mnemonic.ToString();
        //}
    }
}
