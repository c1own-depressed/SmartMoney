namespace MYB_NEW
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using BLL;
    using Presentation;
    
    public partial class AddCategoryPage : Window
    {
        private int categoryID;
        private Main mainPage;
        private Dictionary<Button, StackPanel> categoryExpenseButtonMap = new Dictionary<Button, StackPanel>();

        public AddCategoryPage(Main mainPage)
        {
            if (UserManager.CurrentUser.Language == "ua")
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("uk-UA");
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
            this.InitializeComponent();
            this.mainPage = mainPage;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string categoryTitle = this.CategoryTitleTextBox.Text;

            int userId = UserManager.CurrentUser.Id;

            MainPageLogic.AddExpenseCategory(userId, categoryTitle);
            ExpenseCategoryWithExpenses? temp = MainPageLogic.GetCategoriesAndExpensesByUserId(userId)
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
                FontSize = 25,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Left,
                Height = 50,
            };

            StackPanel categoryListView = new StackPanel();

            Button addCategoryExpenseButton = new Button
            {
                Content = Presentation.Resources.Btn_AddExpense,
                Style = (Style)this.mainPage.Resources["InvisibleButtonStyle"],
                Width = 160,
                FontWeight = FontWeights.Bold,
                FontSize = 20,
            };
            Button editCategoryButton = new Button
            {
                Style = (Style)this.Resources["InvisibleButtonStyle"],
                Width = 30,
                Height = 30,
                Name = $"EditIncomeButton_{Guid.NewGuid():N}",
                VerticalAlignment = VerticalAlignment.Top,
                Content = new TextBlock
                {
                    Text = "E",
                    FontSize = 25,
                    FontWeight = FontWeights.Bold,
                },
            };

            Button deleteCategoryButton = new Button
            {
                Style = (Style)this.Resources["InvisibleButtonStyle"],
                Width = 30,
                Height = 30,
                Name = $"DeleteIncomeButton_{Guid.NewGuid().ToString("N")}",
                VerticalAlignment = VerticalAlignment.Top,
                Content = new TextBlock
                {
                    Text = "D",
                    FontSize = 25,
                    FontWeight = FontWeights.Bold,
                },
            };
            editCategoryButton.Click += (sender, e) => this.EditCategoryButton_Click(sender, e, this.categoryID);
            deleteCategoryButton.Click += (sender, e) => this.DeleteCategoryButton_Click(sender, e, this.categoryID);

            addCategoryExpenseButton.Click += (sender, e) => this.AddCategoryExpenseButton_Click(sender, e, this.categoryID);
            this.categoryExpenseButtonMap.Add(addCategoryExpenseButton, categoryListView);
            StackPanel  categorypanel= new StackPanel { Orientation = Orientation.Horizontal, };

            categoryListView.Children.Add(addCategoryExpenseButton);

            categorypanel.Children.Add(categoryHeader);
            categorypanel.Children.Add(editCategoryButton);
            categorypanel.Children.Add(deleteCategoryButton);
            newCategoryStackPanel.Children.Add(categorypanel);
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

        private void DeleteCategoryButton_Click(object sender, RoutedEventArgs e, int categoryID)
        {
            DeleteCategoryPage deleteCategoryPage = new DeleteCategoryPage(this, categoryID);
            deleteCategoryPage.ShowDialog();
            this.mainPage.UpdateUIAfterCategoryChange();
        }

        private void EditCategoryButton_Click(object sender, RoutedEventArgs e, int categoryID)
        {
            EditCategoryPage editCategoryPage = new EditCategoryPage(this, categoryID);
            editCategoryPage.ShowDialog();
            this.mainPage.UpdateUIAfterCategoryChange();
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
