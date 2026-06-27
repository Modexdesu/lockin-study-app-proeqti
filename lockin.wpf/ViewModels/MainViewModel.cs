using System.Collections.ObjectModel;
using System.Threading.Tasks;
using lockin.core.Interfaces;
using lockin.core.Models;

namespace lockin.wpf.ViewModels
{
    public class MainViewModel
    {
        private readonly ITopicRepository _topicRepository;

        public ObservableCollection<Topic> Topics { get; set; }

        public MainViewModel(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
            Topics = new ObservableCollection<Topic>();

            // The discard operator (_) tells the compiler: 
            // "I intentionally want this task to run in the background 
            // without making the constructor wait for it to finish."
            _ = LoadTopicsAsync();
        }

        // Change this to an async Task method
        private async Task LoadTopicsAsync()
        {
            // 1. Await the database results on a background thread
            var dbTopics = await _topicRepository.GetAllTopicsAsync();

            // 2. Once the data arrives, pop back over to the UI thread to update the collection
            foreach (var topic in dbTopics)
            {
                Topics.Add(topic);
            }
        }
    }
}