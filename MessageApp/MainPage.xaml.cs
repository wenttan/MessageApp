using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MessageApp.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Collections.ObjectModel;
using MessageApp.App_Code;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MessageApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public List<MessageHistory> messageList;

        public MainPage()
        {
            this.InitializeComponent();
            messageList = MessageHistory();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string to = TxtTo.Text;
            string text = TxtMessage.Text;

            if (ValidateInput(to, text))
            {
                string result = string.Empty;
                string[] phoneNumbers = to.Split(',');

                foreach (var number in phoneNumbers)
                {
                    Message message = new Message
                    {
                        Created = DateTime.Now,
                        To = number,
                        MessageText = text
                    };

                    result += SendMessage(message) + Environment.NewLine;
                }


                //Show dialog message
                // Create the message dialog and set its content
                var messageDialog = new MessageDialog(result);
                messageDialog.Commands.Add(new UICommand("Close"));
                messageDialog.DefaultCommandIndex = 0;
                messageDialog.CancelCommandIndex = 1;
                await messageDialog.ShowAsync();

                messageList = MessageHistory();

                myDataGrid.ItemsSource = new ObservableCollection<MessageHistory>(from item in messageList
                                                                                  orderby item.Id descending
                                                                                  select item);
            }

            
        }

        private string SendMessage(Message message)
        {
            string result = string.Empty;

            try
            {
                using(var db = new MessageAppDBContext())
                {
                    //Create message
                    db.Message.Add(message);
                    int rowsAffected = db.SaveChanges();

                    if (rowsAffected > 0)
                    {
                        TwilioAPI twilio = new TwilioAPI();

                        var response = twilio.SendSMS(message);

                        db.MessageResult.Add(response);
                        rowsAffected = db.SaveChanges();

                        if (rowsAffected > 0)
                        {
                            result = response.ConfirmationCode;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        private static List<MessageHistory> MessageHistory()
        {

            try
            {
                using (var db = new MessageAppDBContext())
                {
                    return db.MessageHistory.FromSqlRaw("SELECT a.Id, [Sent], [To], MessageText, ConfirmationCode FROM [Message] a INNER JOIN MessageResult b ON a.Id = b.MessageId")
                                                        .OrderByDescending(a => a.Id).ToList();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void dg_Sorting(object sender, DataGridColumnEventArgs e)
        {
            try
            {
                //Sorting by Sent Column, using the Tag property to pass the bound column name for the sorting implementation 
                if (e.Column.Tag.ToString() == "Sent")
                {
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                    {
                        myDataGrid.ItemsSource = new ObservableCollection<MessageHistory>(from item in messageList
                                                                                              orderby item.Sent ascending
                                                                            select item);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;
                    }
                    else
                    {
                        myDataGrid.ItemsSource = new ObservableCollection<MessageHistory>(from item in messageList
                                                                                              orderby item.Sent descending
                                                                            select item);
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                }
                //Sorting by To Column
                else if (e.Column.Tag.ToString() == "To")
                {
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                    {
                        myDataGrid.ItemsSource = new ObservableCollection<MessageHistory>(from item in messageList
                                                                                              orderby item.To ascending
                                                                                              select item);
                        e.Column.SortDirection = DataGridSortDirection.Ascending;
                    }
                    else
                    {
                        myDataGrid.ItemsSource = new ObservableCollection<MessageHistory>(from item in messageList
                                                                                              orderby item.To descending
                                                                                              select item);
                        e.Column.SortDirection = DataGridSortDirection.Descending;
                    }
                }

                // Removing sorting indicators from other columns
                foreach (var dgColumn in myDataGrid.Columns)
                {
                    if (dgColumn.Tag.ToString() != e.Column.Tag.ToString())
                    {
                        dgColumn.SortDirection = null;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool ValidateInput(string to, string message)
        {
            bool result = true;
            try
            {
                if (string.IsNullOrEmpty(to.Trim()))
                {
                    ToValidator.Text = "Required!";
                    result = false;
                }
                else
                {
                    ToValidator.Text = string.Empty;
                }

                if (string.IsNullOrEmpty(message.Trim()))
                {
                    MessageValidator.Text = "Required!";
                    result = false;
                }
                else
                {
                    ToValidator.Text = string.Empty;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }
    }
}
