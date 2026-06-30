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
        private List<Question> _loadedQuestions;
        public event Action OnLockinStopped;
        public event Action OnLockinStarted;
        public ICommand RelaxCommand { get; set; }

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
            _loadedQuestions= new List<Question>();
            _lockinTimer = new DispatcherTimer();
            _lockinTimer.Tick += LockinTimer_Tick;
            StartGameCommand = new RelayCommand(StartGame);
            RelaxCommand = new RelayCommand(RelaxGame);


            // Wire up all CRUD 
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

        public async void LoadTopicsAsync()
        {
            try
            {
                // 1. Fetch the data from the repo
                var dbTopics = await _topicRepository.GetAllTopicsAsync();

                // 2. Clear out any old items just in case
                Topics.Clear();

                // 3. Pour them into the ObservableCollection to alert the UI
                foreach (var topic in dbTopics)
                {
                    Topics.Add(topic);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error loading topics: {ex.Message}");
            }
        }

        // Your existing notification code below it...
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
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

        private async void StartGame()
        {
            // 1. Pick a category error
            if (SelectedTopic == null)
            {
                MessageBox.Show("Please select a topic category before starting the lock-in.", "Setup Missing");
                return;
            }

            // 2. Clear old questions 
            _loadedQuestions.Clear();

            try
            {
                // 3. Fetch the questions linked to this topic directly from SQLite
                var databaseQuestions = await _topicRepository.GetQuestionsByTopicIdAsync(SelectedTopic.TopicId);


                // 4. Filter by Difficulty (If your Question model has a Difficulty property)
                foreach (var q in databaseQuestions)
                {
                    // Note: If you don't have a Difficulty property on your DB model yet, 
                    // you can comment out this if statement and just do: _loadedQuestions.Add(q);
                   // if (q.Difficulty == GameDifficulty)
                    {
                        _loadedQuestions.Add(q);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load questions from database: {ex.Message}", "Database Error");
                return;
            }

            // 5. Test questions
            if (_loadedQuestions.Count == 0)
            {
                MessageBox.Show("No questions found matching this difficulty level in the database.", "No Data");
                return;
            }

            // 6. Tell the View to minimize the main window
            OnLockinStarted?.Invoke();

            // 7. Start the first countdown
            ScheduleNextPopUp();
        }
        private void LockinTimer_Tick(object sender, EventArgs e)
        {
            // 1. Stop the clock to prevent multiple windows from spawning
            _lockinTimer.Stop();

            // 2. Pick a random question object from your database list
            int randomQuestionIndex = _randomizer.Next(_loadedQuestions.Count);
            Question selectedQuestion = _loadedQuestions[randomQuestionIndex];

            // 3. Instantiate the window and show it as a dialog
            // This stops execution on this line until the user closes the window
            var qWindow = new lockin.wpf.Views.QuestionWindow(selectedQuestion);
            qWindow.ShowDialog();

            // 4. Once they are done, restart the timer
            ScheduleNextPopUp();
        }
        private void ScheduleNextPopUp()
        {
            // Ensure you have _randomizer initialized in your constructor for this to work
            int randomSeconds = _randomizer.Next(5, 16);

            _lockinTimer.Interval = TimeSpan.FromSeconds(randomSeconds);
            _lockinTimer.Start();
        }
        private async void RelaxGame()
        {
            // 1. Kill the timer immediately
            _lockinTimer.Stop();

            // 2. Launch the Relax selection UI
            var relaxWindow = new lockin.wpf.Views.RelaxWindow();

            // ShowDialog blocks execution until they make a choice
            bool? result = relaxWindow.ShowDialog();

            if (result == true)
            {
                if (relaxWindow.IsCompletelyStopped)
                {
                    // Fully terminate session
                    _loadedQuestions.Clear();
                    OnLockinStopped?.Invoke(); // Restores main window visibility
                }
                else
                {
                    // Start the async wait. The app stays responsive while it waits.
                    await Task.Delay(relaxWindow.SelectedDuration);

                    // Resume the game loop
                    ScheduleNextPopUp();
                }
            }
            else
            {
                // User closed the window without clicking - resume anyway
                ScheduleNextPopUp();
            }
        }
    }



    }


