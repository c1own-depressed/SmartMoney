﻿using BLL;
using DAL;
using MYB.DAL;
using MYB_NEW;
using reg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;
            //bool rememberMe = checkBox.IsChecked == true;

            using (var context = new AppDBContext())
            {
                int userId = UserQueries.CheckCredentials(username, password);
                if (Convert.ToBoolean(userId))  //Convert.ToBoolean(userId)
                {
                    InnerUser user = new InnerUser
                    {
                        UserId = userId, 
                    };
                    UserManager.Instance.CurrentUser = user;

                    Main main = new Main();
                    Window.GetWindow(this).Content = main;
                }
                else
                {
                    MessageBox.Show("Invalid username or password. Please try again!");
                }
            }
        }
    }
}
