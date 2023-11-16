﻿using BLL;
using DAL;
using MYB_NEW;
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
namespace OtherPages
{
    /// <summary>
    /// Interaction logic for New_transaction.xaml
    /// </summary>
    public partial class New_transaction : Page
    {
        private int userId;
        private ExpenseCategoryWithExpenses? selectedCategoryWithExpenses;
        public New_transaction()
        {
            InitializeComponent();

            userId =  UserManager.CurrentUser.Id;

            List<ExpenseCategoryWithExpenses> categoriesWithExpenses = NewTransactionLogic.GetCategoriesAndExpensesByUserId(userId);
            foreach (var categoryWithExpenses in categoriesWithExpenses)
            {
                CategoryComboBox.Items.Add(categoryWithExpenses.ExpenseCategory.CategoryName);
            }

          
            CategoryComboBox.SelectionChanged += CategoryComboBox_SelectionChanged;

            
            ExpenseComboBox.IsEnabled = false;
            CostTextBox.IsEnabled = false;

        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ExpenseComboBox.Items.Clear();

            if (CategoryComboBox.SelectedItem != null)
            {
                
                string selectedCategory = (CategoryComboBox?.SelectedItem?.ToString() ?? "Unknown");


                selectedCategoryWithExpenses = NewTransactionLogic.GetCategoriesAndExpensesByUserId(userId)
                    .FirstOrDefault(category => category.ExpenseCategory.CategoryName == selectedCategory);

              
                if (selectedCategoryWithExpenses != null)
                {
                    foreach (var expense in selectedCategoryWithExpenses.Expenses)
                    {
                        ComboBoxItem comboBoxItem = new ComboBoxItem();
                        comboBoxItem.Content = expense.ExpenseName;
                        ExpenseComboBox.Items.Add(comboBoxItem);
                    }
                }

                
                ExpenseComboBox.IsEnabled = true;
                CostTextBox.IsEnabled = true;
            }
            else
            {
                CostTextBox.IsEnabled = false;
                ExpenseComboBox.IsEnabled = false;
            }
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Main main = new Main();
            Window.GetWindow(this).Content = main;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem expenseItem = (ComboBoxItem)ExpenseComboBox.SelectedItem;
            string expenseName = (expenseItem?.Content?.ToString() ?? "Unknown");
            var expense = selectedCategoryWithExpenses.Expenses.FirstOrDefault(expense => expense.ExpenseName == expenseName);
            int cost = Convert.ToInt32(CostTextBox.Text);
            string transactionName = TransactionTextBox.Text;
            NewTransactionLogic.AddTransaction(transactionName, cost, expense.Id);
        }
    }
}
