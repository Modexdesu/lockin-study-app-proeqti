using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using lockin.core.Interfaces;
using lockin.core.Models;
using lockin.wpf.Commands;

namespace lockin.wpf.ViewModels
{
    public class MainViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private readonly ITopicRepository _topicRepository;
        private string _newTopicName;
        private string _editTopicName;
        private Topic _selectedTopic;
        private int _gameDifficulty = 1; // Default to 1 (Easy)
        private DispatcherTimer _lockinTimer;
        private Random _randomizer;
        private List<string> _loadedQuestions;
        public event Action OnLockinStarted;

        public ObservableCollection<Topic> Topics { get; set; }

        // CRUD Commands
        public ICommand AddTopicCommand { get; set; }
        public ICommand UpdateTopicCommand { get; set; }
        public ICommand DeleteTopicCommand { get; set; }
        public ICommand StartGameCommand { get; set; }

        // Bindable properties
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

                // CRUCIAL LINK: If an item is selected in the table, update its name immediately
                if (SelectedTopic != null)
                {
                    SelectedTopic.TopicName = value; 
                }
            }

        }


        public Topic SelectedTopic
        {
            get => _selectedTopic;
            set
            {
                _selectedTopic = value;
                OnPropertyChanged(nameof(SelectedTopic));

                // When an item is selected, populate the Edit field automatically
                EditTopicName = _selectedTopic?.TopicName ?? string.Empty;
            }
        }

        public MainViewModel(ITopicRepository topicRepository)
        {


            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TriviaQuestions");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            _topicRepository = topicRepository;
            Topics = new ObservableCollection<Topic>();


            _randomizer= new Random();
            _loadedQuestions= new List<string>();
            _lockinTimer = new DispatcherTimer();
            _lockinTimer.Tick += LockinTimer_Tick;
            // Wire up all CRUD endpoints
            AddTopicCommand = new RelayCommand(AddTopic);
            UpdateTopicCommand = new RelayCommand(UpdateTopic);
            DeleteTopicCommand = new RelayCommand(DeleteTopic);
            StartGameCommand = new RelayCommand(StartGame);
        }

        // === CREATE ===
        private async void AddTopic()
        {
            if (string.IsNullOrWhiteSpace(NewTopicName)) return;

            var newTopic = new Topic { TopicName = NewTopicName };
            try
            {
                await _topicRepository.AddTopicAsync(newTopic);
                Topics.Add(newTopic);

                // Auto-generate target folder and TXT template
                string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TriviaQuestions");
                Directory.CreateDirectory(directoryPath);
                string safeFileName = string.Join("_", newTopic.TopicName.Split(Path.GetInvalidFileNameChars()));
                string filePath = Path.Combine(directoryPath, $"{safeFileName}_Questions.txt");

                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, $"# Questions for {newTopic.TopicName}\n# Format: Question|Ans1|Ans2|Ans3|CorrectAns|DifficultyId(1-3)\n");
                }

                NewTopicName = string.Empty;
            }
            catch (Exception ex) { MessageBox.Show($"Create failed: {ex.Message}"); }
        }

        // === UPDATE ===
        private async void UpdateTopic()
        {
            if (SelectedTopic == null || string.IsNullOrWhiteSpace(EditTopicName)) return;

            try
            {
                // 1. Update local memory reference values
                SelectedTopic.TopicName = EditTopicName;

                // 2. Pass to repository framework to run SQL UPDATE statement
                await _topicRepository.UpdateTopicAsync(SelectedTopic);

                // 3. Force WPF UI list element collection to visually refresh instantly
                // Notice I used "Topics" here to match your collection name
                System.Windows.Data.CollectionViewSource.GetDefaultView(Topics).Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Update failed: {ex.Message}");
            }
        }

        // === DELETE ===
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

        public async Task GetAllTopicsAsync()
        {
            var dbTopics = await _topicRepository.GetAllTopicsAsync();
            foreach (var topic in dbTopics) Topics.Add(topic);
        }

        public event System.ComponentModel.PropertyChangedEventHandler  PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));

       

        public int GameDifficulty
        {
            get => _gameDifficulty;
            set
            {
                _gameDifficulty = value;
                OnPropertyChanged(nameof(GameDifficulty));
            }
        }

        private void StartGame()
        {
            // 1.pick a category error
            if (SelectedTopic == null)
            {
                MessageBox.Show("Please select a topic category before starting the lock-in.", "Setup Missing");
                return;
            }

            // 2.  find the specific question file for this topic
            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TriviaQuestions");
            string safeFileName = string.Join("_", SelectedTopic.TopicName.Split(Path.GetInvalidFileNameChars()));
            string filePath = Path.Combine(directoryPath, $"{safeFileName}_Questions.txt");

            if (!File.Exists(filePath))
            {
                MessageBox.Show("The question file for this topic is missing.", "File Error");
                return;
            }

            // 3. Clear old questions 
            _loadedQuestions.Clear();

            // 4. Read the text file and filter by Difficulty
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                // Skip comment lines or empty lines
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                // Split the row by the  character
                string[] segments = line.Split('|');
                if (segments.Length >= 6 && int.TryParse(segments[5], out int difficulty))
                {
                    if (difficulty == GameDifficulty)
                    {
                        // Store the full valid question string in memory
                        _loadedQuestions.Add(line);
                    }
                }
            }

            // 5.test questions
            if (_loadedQuestions.Count == 0)
            {
                MessageBox.Show("No questions found matching this difficulty level in the text file.", "No Data");
                return;
            }

            // 6. Tell the View to minimize the main window
            OnLockinStarted?.Invoke();

            // 7. Start the first countdown
            ScheduleNextPopUp();

        }

        public void ScheduleNextPopUp()
        {
            // Set random interval between 5 and 15 seconds for testing
            int randomSeconds = _randomizer.Next(5, 16);

            _lockinTimer.Interval = TimeSpan.FromSeconds(randomSeconds);
            _lockinTimer.Start();
        }

        private void LockinTimer_Tick(object sender, EventArgs e)
        {
            // 1. Stop timer 
            _lockinTimer.Stop();

            // 2. Pick a random question
            int randomQuestionIndex = _randomizer.Next(_loadedQuestions.Count);
            string selectedQuestionRow = _loadedQuestions[randomQuestionIndex];

            // 3. Temporary test pop-up (Will be replaced in Phase 2)
            MessageBox.Show($"POP UP TRIGGERED!\n\nQuestion Data:\n{selectedQuestionRow}", "Hostage Situation");

            // 4. Restart loop after they click OK
            ScheduleNextPopUp();
        }

    }


}