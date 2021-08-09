using MySql.Data.MySqlClient;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BankForm
{
    public partial class BankForm : Form
    {
        Database database = new Database("localhost", "bank", "root", "root");
        MySqlConnection connection;

        bool mouseDown;
        Point offset;

        public BankForm()
        {
            InitializeComponent();
            MoneyLabel.Visible = false;
            database.Connect();
            connection = database.GetConnection();
        }

        private void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            offset.X = e.X;
            offset.Y = e.Y;
            mouseDown = true;
        }

        private void Panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Point currentScreenPos = PointToScreen(e.Location);
                Location = new Point(currentScreenPos.X - offset.X, currentScreenPos.Y - offset.Y);
            }
        }

        private void Panel_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void Exit_MouseClick(object sender, MouseEventArgs e)
        {
            Close();
        }


        private void LoginButton_Click(object sender, System.EventArgs e)
        {
            MySqlCommand selectQuery = new MySqlCommand()
            {
                Connection = connection,
                CommandText = $"SELECT IFNULL(COUNT(*),0) FROM bank.user WHERE Name='{NameInput.Text}' AND Password='{GetHashString(PasswordInput.Text)}';"
            };

            connection.Open();
            MySqlDataReader reader;
            reader = selectQuery.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    if (reader.GetString(0) == "1")
                    {
                        WarningLabel.Text = "Welcome " + NameInput.Text;
                        MoneyLabel.Visible = true;
                    }
                    else
                    {
                        WarningLabel.Text = "Wrong Name or Password";
                        ButtonBackWait(LoginButton);
                        WarningClear();
                    }
                }
            } finally {
                reader.Close();
                connection.Close();
            }

            try
            {
                connection.Open();
                selectQuery = new MySqlCommand()
                {
                    Connection = connection,
                    CommandText = $"SELECT Money FROM bank.user WHERE Name='{NameInput.Text}';"
                };
                reader = selectQuery.ExecuteReader();
                while (reader.Read())
                {
                    MoneyLabel.Text = "Money: " + reader.GetString("Money");
                }
            } finally { 
                reader.Close();
                connection.Close();
            }
        }

        private void RegisterButton_Click(object sender, System.EventArgs e)
        {

            MySqlCommand cmd = new MySqlCommand() {
                Connection = connection,
                CommandText = $"INSERT INTO user(Name, Password) VALUES('{NameInput.Text}', '{GetHashString(PasswordInput.Text)}');"
            };

            connection.Open();
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch
            {
                ButtonBackWait(RegisterButton);
                WarningLabel.Text = "User already exists";
            } finally
            {
                connection.Close();
                WarningClear();
            }
        }

        public async void ButtonBackWait(Button button)
        {
            button.BackColor = ColorTranslator.FromHtml("#e74c3c");
            await Task.Delay(250);
            button.BackColor = ColorTranslator.FromHtml("#16a085");
        }

        public async void WarningClear()
        {
            await Task.Delay(1000);
            WarningLabel.Text = "";
        }

        public string GetHashString(string text)
        {

            HashAlgorithm sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
