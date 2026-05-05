using System.Windows;
using UIN.Library.WPF.Services;

namespace UIN.Library.WPF
{
    public partial class MainWindow : Window
    {
        private ApiService _api = new ApiService();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var username = UsernameBox.Text;
            var role = RoleBox.Text;

            var success = await _api.Login(username, role);

            if (success)
                MessageBox.Show("Login réussi !");
            else
                MessageBox.Show("Erreur login");
        }

        private async void LoadBooks_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var books = await _api.GetBooks();
                BooksList.ItemsSource = books;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void Logout_Click(object sender, RoutedEventArgs e)
        {
            await _api.Logout();

            MessageBox.Show("Déconnecté !");

            BooksList.ItemsSource = null;
        }
    }
}