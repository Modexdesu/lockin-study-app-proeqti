using lockin.core.Interfaces;
using lockin.core.Models;
using lockin.wpf.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace lockin.wpf.ViewModels
{
    public class MainViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private readonly ITopicRepository _topicRepository;
        private readonly IUserRepository _userRepository;

        private string _newTopicName;
        private string _editTopicName;
        private Topic _selectedTopic;
        private int _gameDifficulty = 1;
        private DispatcherTimer _lockinTimer;
        private Random _randomizer;
        private List<Question> _loadedQuestions;
        public event Action OnLockinStopped;
        public event Action OnLockinStarted;
        private UserInfo _currentUser;

        public UserInfo CurrentUser
        {
            get => _currentUser;
            set { _currentUser = value; OnPropertyChanged(nameof(CurrentUser)); OnPropertyChanged(nameof(CurrentLevel)); }
        }

        // Level is okay to leave here as a display property
        public int CurrentLevel => (CurrentUser?.Xp / 100 ?? 0) + 1;

        public ObservableCollection<Topic> Topics { get; set; }

        public ICommand RelaxCommand { get; set; }
        public ICommand AddTopicCommand { get; set; }
        public ICommand UpdateTopicCommand { get; set; }
        public ICommand DeleteTopicCommand { get; set; }
        public ICommand StartGameCommand { get; set; }

        public string NewTopicName
        {
            get => _newTopicName;
            set { _newTopicName = value; OnPropertyChanged(nameof(NewTopicName)); }
        }

        public string EditTopicName
        {
            get => _editTopicName;
            set
            {
                _editTopicName = value;
                OnPropertyChanged(nameof(EditTopicName));
                if (SelectedTopic != null) SelectedTopic.TopicName = value;
            }
        }

        public Topic SelectedTopic
        {
            get => _selectedTopic;
            set
            {
                _selectedTopic = value;
                OnPropertyChanged(nameof(SelectedTopic));
                EditTopicName = _selectedTopic?.TopicName ?? string.Empty;
            }
        }

        public int GameDifficulty
        {
            get => _gameDifficulty;
            set { _gameDifficulty = value; OnPropertyChanged(nameof(GameDifficulty)); }
        }

        public MainViewModel(ITopicRepository topicRepository, IUserRepository userRepository)
        {
            _topicRepository = topicRepository;
            _userRepository = userRepository;
            Topics = new ObservableCollection<Topic>();
            _randomizer = new Random();
            _loadedQuestions = new List<Question>();

            _lockinTimer = new DispatcherTimer();
            _lockinTimer.Tick += LockinTimer_Tick;

            AddTopicCommand = new RelayCommand(AddTopic);
            UpdateTopicCommand = new RelayCommand(UpdateTopic);
            DeleteTopicCommand = new RelayCommand(DeleteTopic);
            StartGameCommand = new RelayCommand(StartGame);
            RelaxCommand = new RelayCommand(RelaxGame);
        }

        private async void AddTopic()
        {
            if (string.IsNullOrWhiteSpace(NewTopicName)) return;

            var newTopic = new Topic { TopicName = NewTopicName };
            try
            {
                // The ViewModel just asks the Repo to do the work.
                await _topicRepository.AddTopicAsync(newTopic);
                Topics.Add(newTopic);
                NewTopicName = string.Empty;
            }
            catch (Exception ex) { MessageBox.Show($"Create failed: {ex.Message}"); }
        }

        private async void UpdateTopic()
        {
            if (SelectedTopic == null || string.IsNullOrWhiteSpace(EditTopicName)) return;
            try
            {
                SelectedTopic.TopicName = EditTopicName;
                await _topicRepository.UpdateTopicAsync(SelectedTopic);
                System.Windows.Data.CollectionViewSource.GetDefaultView(Topics).Refresh();
            }
            catch (Exception ex) { MessageBox.Show($"Update failed: {ex.Message}"); }
        }

        private async void DeleteTopic()
        {
            if (SelectedTopic == null) return;
            try
            {
                await _topicRepository.DeleteTopicAsync(SelectedTopic);
                Topics.Remove(SelectedTopic);
            }
            catch (Exception ex) { MessageBox.Show($"Delete failed: {ex.Message}"); }
        }

        public async void LoadTopicsAsync()
        {
            try
            {
                var dbTopics = await _topicRepository.GetAllTopicsAsync();
                Topics.Clear();
                foreach (var topic in dbTopics) Topics.Add(topic);
            }
            catch (Exception ex) { MessageBox.Show($"Error loading topics: {ex.Message}"); }
        }

        private async void StartGame()
        {
            if (SelectedTopic == null)
            {
                MessageBox.Show("Please select a topic category.", "Setup Missing");
                return;
            }

            _loadedQuestions.Clear();

            try
            {
                // The Repo should handle the difficulty filtering, not the ViewModel
                var databaseQuestions = await _topicRepository.GetQuestionsAsync(SelectedTopic.TopicId, GameDifficulty);
                _loadedQuestions.AddRange(databaseQuestions);
            }
            catch (Exception ex) { MessageBox.Show($"Database Error: {ex.Message}"); return; }

            if (_loadedQuestions.Count == 0)
            {
                MessageBox.Show("No questions found.", "No Data");
                return;
            }

            OnLockinStarted?.Invoke();
            ScheduleNextPopUp();
        }

        private async void LockinTimer_Tick(object sender, EventArgs e)
        {
            _lockinTimer.Stop();

            // Safety check: Empty Ammo
            if (_loadedQuestions.Count == 0)
            {
                MessageBox.Show("You have answered all questions in this topic.", "Session Complete");
                OnLockinStopped?.Invoke();
                return;
            }

            int randomQuestionIndex = _randomizer.Next(_loadedQuestions.Count);
            Question selectedQuestion = _loadedQuestions[randomQuestionIndex];

            var qWindow = new lockin.wpf.Views.QuestionWindow(selectedQuestion);
            bool? isCorrect = qWindow.ShowDialog();

            // Let the Database Repo handle the math
            _userRepository.ProcessAnswerResult(CurrentUser.UserId, isCorrect == true, selectedQuestion.TopicId);

            // Reload user to update UI
            CurrentUser = await _userRepository.GetCurrentUserAsync();

            // Remove the question so it doesn't repeat
            _loadedQuestions.RemoveAt(randomQuestionIndex);

            ScheduleNextPopUp();
        }

        private void ScheduleNextPopUp()
        {
            int randomSeconds = _randomizer.Next(5, 16);
            _lockinTimer.Interval = TimeSpan.FromSeconds(randomSeconds);
            _lockinTimer.Start();
        }

        private async void RelaxGame()
        {
            _lockinTimer.Stop();
            var relaxWindow = new lockin.wpf.Views.RelaxWindow();
            bool? result = relaxWindow.ShowDialog();

            if (result == true)
            {
                if (relaxWindow.IsCompletelyStopped)
                {
                    _loadedQuestions.Clear();
                    OnLockinStopped?.Invoke();
                }
                else
                {
                    await Task.Delay(relaxWindow.SelectedDuration);
                    ScheduleNextPopUp();
                }
            }
            else
            {
                ScheduleNextPopUp();
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
    }
}