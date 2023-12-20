using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using IST_Password_app.Infrastructure.Commands;
using IST_Password_app.ViewModel.Base;

namespace IST_Password_app.ViewModel
{
    internal class MainViewModel : BaseViewModel
    {
        private string _passwordExample;
        private string _password;
        private Visibility _showAuthenticatedLabel;
        private bool _isPasswordFieldEnabled;
        private bool _buttonAllowed;
        private Timer _inputTimer;
        private Timer _inputProhibitionTimer;
        private Timer _passwordTimer;

        public string PasswordExample
        {
            get => _passwordExample;
            set
            {
                _passwordExample = value;
                OnPropertyChanged(nameof(PasswordExample));

                ShowAuthenticatedLabel = Visibility.Hidden;

                StartNewPasswordTimer();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));

                StartNewInputTimer();
            }
        }

        public Visibility ShowAuthenticatedLabel
        {
            get => _showAuthenticatedLabel;
            set
            {
                _showAuthenticatedLabel = value;
                OnPropertyChanged(nameof(ShowAuthenticatedLabel));
            }
        }

        public bool IsPasswordFieldEnabled 
        { 
            get => _isPasswordFieldEnabled;
            set
            {
                _isPasswordFieldEnabled = value;
                OnPropertyChanged(nameof(IsPasswordFieldEnabled));
            }
        }

        /*-----------------------------------------------------------------------------------------*/

        #region Commands



        #region Authenticate Command

        public ICommand AuthenticateCommand { get; set; }

        private bool CanAuthenticateCommandExecute(object p)
        {
            if (string.IsNullOrEmpty(PasswordExample)
                || string.IsNullOrEmpty(Password)
                || !_buttonAllowed)
            {
                return false;
            }
            return true;
        }

        private void OnAuthenticateCommandExecuted(object p)
        {
            if (PasswordExample == Password)
            {
                ShowAuthenticatedLabel = Visibility.Visible;
            }
        }

        #endregion



        #endregion

        /*-----------------------------------------------------------------------------------------*/

        #region Events        

        public void EndInputTimeEvent(object source, ElapsedEventArgs e)
        {
            _inputTimer = null;
            _buttonAllowed = false;
            IsPasswordFieldEnabled = false;
           StartNewInputProhibitionTimer();
        }

        public void EndInputProhibitionTimerEvent(object source, ElapsedEventArgs e) 
        {
            _inputProhibitionTimer = null;
            IsPasswordFieldEnabled = true;
            _buttonAllowed = true;
        }

        public void EndPasswordTimerEvent(object source, ElapsedEventArgs e)
        {
            _passwordTimer = null;
            _buttonAllowed = false;
            IsPasswordFieldEnabled = false;
            _inputProhibitionTimer = null;
        }

        #endregion

        /*-----------------------------------------------------------------------------------------*/

        private void StartNewInputTimer() 
        {
            _inputTimer = new Timer
            {
                //Interval = TimeSpan.FromMinutes(2).TotalMilliseconds,
                Interval = 10000,
                AutoReset = false
            };
            _inputTimer.Elapsed += new ElapsedEventHandler(EndInputTimeEvent);
            _inputTimer.Enabled = true;
        }

        private void StartNewInputProhibitionTimer()
        {
            _inputProhibitionTimer = new Timer
            {
                //Interval = TimeSpan.FromMinutes(5).TotalMilliseconds,
                Interval = 25000,
                AutoReset = false
            };
            _inputProhibitionTimer.Elapsed += new ElapsedEventHandler(EndInputProhibitionTimerEvent);
            _inputProhibitionTimer.Enabled = true;
        }

        private void StartNewPasswordTimer()
        {
            _passwordTimer = new Timer
            {
                //Interval = TimeSpan.FromMinutes(10).TotalMilliseconds,
                Interval = TimeSpan.FromMinutes(1).TotalMilliseconds,
                AutoReset = false
            };
            _passwordTimer.Elapsed += new ElapsedEventHandler(EndPasswordTimerEvent);
            _passwordTimer.Enabled = true;
            _buttonAllowed = true;
            IsPasswordFieldEnabled = true;
        }

        /*-----------------------------------------------------------------------------------------*/

        public MainViewModel()
        {
            IsPasswordFieldEnabled = true;
            _buttonAllowed = true;
            ShowAuthenticatedLabel = Visibility.Hidden;

            StartNewInputTimer();
            StartNewInputProhibitionTimer();
            StartNewPasswordTimer();

            AuthenticateCommand = new LambdaCommand(OnAuthenticateCommandExecuted, CanAuthenticateCommandExecute);
        }
    }
}
