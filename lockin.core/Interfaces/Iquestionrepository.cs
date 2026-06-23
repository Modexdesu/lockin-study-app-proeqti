using lockin.core.models;

namespace lockin.core.Interfaces
{
    public interface Iquestionrepository
    {
        List<Question> GetAll();
        Question GetById(int id);
        void Add (Question question);
        void Update (Question question);
        void Delete (int id);
    }
}
