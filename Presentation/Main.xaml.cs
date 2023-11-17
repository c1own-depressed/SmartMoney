﻿namespace MYB_NEW
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using BLL;
    using DAL;
    using OtherPages;
    using Presentation;
    using reg;

    public partial class Main : Page
    {
        public Main()
        {
            this.InitializeComponent();
            int userId = UserManager.CurrentUser.Id;

            List<Income> incomes = MainPageLogic.GetIncomesByUserId(userId);
            List<Saving> savings = MainPageLogic.GetSavingsByUserId(userId);
            List<ExpenseCategoryWithExpenses> expenses = MainPageLogic.GetCategoriesAndExpensesByUserId(userId);

            for (int i = 0; i < incomes.Count; i++)
            {
                this.IncomeListView.Children.Add(new TextBlock() { Text = incomes[i].IncomeName, FontSize = 40, FontWeight = FontWeights.DemiBold });
            }

            for (int i = 0; i < savings.Count; i++)
            {
                this.SavingsListView.Children.Add(new TextBlock() { Text = savings[i].SavingName, FontSize = 40, FontWeight = FontWeights.DemiBold });
            }

            this.UsernameTextBlock.Text = UserManager.CurrentUser.Username;

            this.SetCategoriesList(expenses);
        }

        private void SetCategoriesList(List<ExpenseCategoryWithExpenses> expenses)
        {
            for (int i = 0; i < expenses.Count; i++)
            {
                int categoryID = expenses[i].ExpenseCategory.Id;
                Dictionary<Button, StackPanel> categoryExpenseButtonMap = new Dictionary<Button, StackPanel>();
                Border newCategoryBlock = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0),
                    Width = 360,
                    HorizontalAlignment = HorizontalAlignment.Left,
                };

                StackPanel newCategoryStackPanel = new StackPanel();

                TextBlock categoryHeader = new TextBlock
                {
                    Text = expenses[i].ExpenseCategory.CategoryName,
                    FontSize = 40,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Height = 100,
                };

                StackPanel categoryListView = new StackPanel();
                foreach (var expense in expenses[i].Expenses)
                {
                    StackPanel newExpensePanel = new StackPanel();
                    newExpensePanel.Orientation = Orientation.Horizontal;

                    TextBlock newExpenseTitle = new TextBlock
                    {
                        Text = expense.ExpenseName,
                        FontSize = 20,
                        FontWeight = FontWeights.DemiBold,
                    };

                    TextBlock spaceText = new TextBlock
                    {
                        Text = " ",
                        FontSize = 10,
                        FontWeight = FontWeights.DemiBold,
                        Foreground = Brushes.Gray,
                    };

                    TextBlock newExpenseBudget = new TextBlock
                    {
                        Text = $"0/{expense.Amount} $",
                        FontSize = 20,
                        FontWeight = FontWeights.DemiBold,
                        Foreground = Brushes.Gray,
                        Height = 20,
                    };

                    newExpensePanel.Children.Add(newExpenseTitle);
                    newExpensePanel.Children.Add(spaceText);
                    newExpensePanel.Children.Add(newExpenseBudget);

                    categoryListView.Children.Add(newExpensePanel);
                }

                Button addCategoryExpenseButton = new Button
                {
                    Content = "Add Expense",
                    Style = (Style)this.Resources["InvisibleButtonStyle"],
                    Width = 360,
                    FontWeight = FontWeights.Bold,
                    FontSize = 30,
                };

                addCategoryExpenseButton.Click += (sender, e) =>
                {
                    Button clickedButton = (Button)sender;

                    if (categoryExpenseButtonMap.ContainsKey(clickedButton))
                    {
                        StackPanel expenseCategory = categoryExpenseButtonMap[clickedButton];
                        AddExpensePage addExpensePage = new AddExpensePage(expenseCategory, categoryID);
                        addExpensePage.ShowDialog();
                    }
                };
                categoryExpenseButtonMap.Add(addCategoryExpenseButton, categoryListView);

                categoryListView.Children.Add(addCategoryExpenseButton);

                newCategoryStackPanel.Children.Add(categoryHeader);
                newCategoryStackPanel.Children.Add(categoryListView);
                newCategoryBlock.Child = newCategoryStackPanel;

                // Find the last category and insert the new one below it
                var lastCategory = this.CategoriesListView.Children.OfType<Border>().LastOrDefault();
                if (lastCategory != null)
                {
                    int insertIndex = this.CategoriesListView.Children.IndexOf(lastCategory) + 1;
                    this.CategoriesListView.Children.Insert(insertIndex, newCategoryBlock);
                }
                else
                {
                    this.CategoriesListView.Children.Add(newCategoryBlock);
                }


                for (int j = 0; j < expenses[i].Expenses.Count; j++)
                {
                    // Створіть новий об'єкт "Expense"
                    Expense newExpense = new Expense(expenses[i].Expenses[j].ExpenseName, expenses[i].Expenses[j].Amount, categoryListView);

                    // Додайте новий "Expense" до категорії витрат (StackPanel)
                    if (categoryListView != null)
                    {
                        StackPanel newExpensePanel = new StackPanel();
                        newExpensePanel.Orientation = Orientation.Horizontal;

                        // Назва витрати
                        TextBlock newExpenseTitle = new TextBlock
                        {
                            Text = newExpense.Title,
                            FontSize = 30,
                            FontWeight = FontWeights.DemiBold,
                        };

                        // Пробіл
                        TextBlock spaceText = new TextBlock
                        {
                            Text = " ",
                            FontSize = 10,
                            FontWeight = FontWeights.DemiBold,
                            Foreground = Brushes.Gray,
                        };

                        // Бюджет
                        TextBlock newExpenseBudget = new TextBlock
                        {
                            Text = $"0/{newExpense.Amount} $",
                            FontSize = 32,
                            FontWeight = FontWeights.DemiBold,
                            Foreground = Brushes.Gray,
                            Height = 32,
                        };

                        // Додайте назву витрати, пробіл і бюджет в StackPanel
                        newExpensePanel.Children.Add(newExpenseTitle);
                        newExpensePanel.Children.Add(spaceText);
                        newExpensePanel.Children.Add(newExpenseBudget);

                        // Знайдіть кнопку "Add Expense" та вставте нову витрату перед нею
                        // for (int i = 0; i < expenseCategory.Children.Count; i++)
                        // {
                        //    if (expenseCategory.Children[i] is Button addExpenseButton)
                        //    {
                        //        expenseCategory.Children.Insert(i, newExpensePanel);
                        //        break;
                        //    }
                        // }
                    }
                }
            }
        }

        private void AddIncome_Click(object sender, RoutedEventArgs e)
        {
            AddIncomePage addIncomePage = new AddIncomePage(this.IncomeListView);
            addIncomePage.ShowDialog();
        }

        private void AddSavings_Click(object sender, RoutedEventArgs e)
        {
            AddSavingsPage addSavingsPage = new AddSavingsPage(this.SavingsListView);
            addSavingsPage.ShowDialog();
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            AddCategoryPage addCategoryPage = new AddCategoryPage(this); // Pass 'this' as the 'mainPage' argument
            addCategoryPage.ShowDialog();
        }

        public void AddCategory(Border newCategory)
        {
            // Додайте нову категорію на сторінку "Main" над кнопкою "Add Category"
            this.CategoriesListView.Children.Insert(this.CategoriesListView.Children.Count - 1, newCategory);
        }

        private void AddTransaction_Click(object sender, RoutedEventArgs e)
        {
            // Тут ви можете додати функціональність для кнопки "Add Transaction"
        }

        private bool buttonsVisible = false;

        private void Toggle_Click(object sender, RoutedEventArgs e)
        {
            if (this.buttonsVisible)
            {
                // Приховати кнопки "Statistic", "Data export", "Tips and Tricks", "Settings" і "Sign out"
                // this.StatisticButton.Visibility = Visibility.Collapsed;
                this.DataExportButton.Visibility = Visibility.Collapsed;
                this.TipsAndTricksButton.Visibility = Visibility.Collapsed;
                this.SettingsButton.Visibility = Visibility.Collapsed;
                this.SignOutButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Показати кнопки "Statistic", "Data export", "Tips and Tricks", "Settings" і "Sign out"
                // this.StatisticButton.Visibility = Visibility.Visible;
                this.DataExportButton.Visibility = Visibility.Visible;
                this.TipsAndTricksButton.Visibility = Visibility.Visible;
                this.SettingsButton.Visibility = Visibility.Visible;
                this.SignOutButton.Visibility = Visibility.Visible;
            }

            this.buttonsVisible = !this.buttonsVisible;
        }

        private void EditIncome_Click(object sender, RoutedEventArgs e)
        {
            // Додайте реалізацію для обробки події редагування доходів
        }

        private void DeleteIncome_Click(object sender, RoutedEventArgs e)
        {
            // Додайте реалізацію для обробки події редагування доходів
        }

        private void EditSavings_Click(object sender, RoutedEventArgs e)
        {
            // Додайте реалізацію для обробки події редагування доходів
        }

        private void DeleteSavings_Click(object sender, RoutedEventArgs e)
        {
            // Додайте реалізацію для обробки події редагування доходів
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            // Додайте реалізацію для обробки події редагування доходів
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            // Додайте реалізацію для обробки події редагування доходів
        }

        private void EditExpense_Click(object sender, RoutedEventArgs e)
        {
            // Додайте реалізацію для обробки події редагування доходів
        }

        private void DeleteExpense_Click(object sender, RoutedEventArgs e)
        {
            // Додайте реалізацію для обробки події редагування доходів
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Windows[0].Close();
        }

        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            // UserManager.Instance.LogOutUser();
            Login login = new Login();
            Window.GetWindow(this).Content = login;
        }

        private void DataExportButton_Click(object sender, RoutedEventArgs e)
        {
            ExportDataPage export_data = new ExportDataPage();
            Window.GetWindow(this).Content = export_data;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsPageView settingsPage = new SettingsPageView();
            Window.GetWindow(this).Content = settingsPage;
        }

        private void TransactionButton_Click(object sender, RoutedEventArgs e)
        {
            New_transaction new_transaction = new New_transaction();
            Window.GetWindow(this).Content = new_transaction;
        }

        private void TipsAndTricksButton_Click(object sender, RoutedEventArgs e)
        {
            TipsAndTricksPage tipsAndTricksPage = new TipsAndTricksPage();
            Window.GetWindow(this).Content = tipsAndTricksPage;
        }

        private void StatisticButton_Click(object sender, RoutedEventArgs e)
        {
            Statistic1 statisticPage = new Statistic1();
            Window.GetWindow(this).Content = statisticPage;
        }
    }
}