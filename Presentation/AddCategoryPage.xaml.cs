﻿namespace MYB_NEW
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using BLL;

    public partial class AddCategoryPage : Window
    {
        private int categoryID;
        private Main mainPage;
        private Dictionary<Button, StackPanel> categoryExpenseButtonMap = new Dictionary<Button, StackPanel>();

        public AddCategoryPage(Main mainPage)
        {
            this.InitializeComponent();
            this.mainPage = mainPage;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string categoryTitle = this.CategoryTitleTextBox.Text;

            int userId = UserManager.CurrentUser.Id;

            MainPageLogic.AddExpenseCategory(userId, categoryTitle);
            ExpenseCategoryWithExpenses? temp = NewTransactionLogic.GetCategoriesAndExpensesByUserId(userId)
                    .FirstOrDefault(category => category.ExpenseCategory.CategoryName == categoryTitle);
            this.categoryID = temp.ExpenseCategory.Id;
            // Create a new category block
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
                Foreground = (SolidColorBrush)Application.Current.Resources["Text"],
                Text = categoryTitle,
                FontSize = 40,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 100,
            };

            StackPanel categoryListView = new StackPanel();

            Button addCategoryExpenseButton = new Button
            {
                Content = "Add Expense",
                Style = (Style)this.mainPage.Resources["InvisibleButtonStyle"],
                Width = 360,
                FontWeight = FontWeights.Bold,
                FontSize = 30,
            };

            addCategoryExpenseButton.Click += (sender, e) => this.AddCategoryExpenseButton_Click(sender, e, this.categoryID);
            this.categoryExpenseButtonMap.Add(addCategoryExpenseButton, categoryListView);

            categoryListView.Children.Add(addCategoryExpenseButton);

            newCategoryStackPanel.Children.Add(categoryHeader);
            newCategoryStackPanel.Children.Add(categoryListView);
            newCategoryBlock.Child = newCategoryStackPanel;

            // Find the last category and insert the new one below it
            var lastCategory = this.mainPage.CategoriesListView.Children.OfType<Border>().LastOrDefault();
            if (lastCategory != null)
            {
                int insertIndex = this.mainPage.CategoriesListView.Children.IndexOf(lastCategory) + 1;
                this.mainPage.CategoriesListView.Children.Insert(insertIndex, newCategoryBlock);
            }
            else
            {
                this.mainPage.CategoriesListView.Children.Add(newCategoryBlock);
            }

            this.Close();
        }

        private void AddCategoryExpenseButton_Click(object sender, RoutedEventArgs e, int categoryID)
        {
            Button clickedButton = (Button)sender;

            if (this.categoryExpenseButtonMap.ContainsKey(clickedButton))
            {
                StackPanel expenseCategory = this.categoryExpenseButtonMap[clickedButton];
                AddExpensePage addExpensePage = new AddExpensePage(expenseCategory,categoryID);
                addExpensePage.ShowDialog();
            }
        }
    }
}