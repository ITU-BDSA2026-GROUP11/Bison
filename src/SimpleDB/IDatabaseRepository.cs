namespace SimpleDB;
public interface IDatabaseRepository<T>
{
public IEnumerable<T> Read(int? limit = null);
public void Store(T record);

public void setFilePath(string fileType);

public Boolean doesIdExist(string ID);

public IEnumerable<Comment> getCommentsUsingId(int id, int? limit = null);

public IEnumerable<Observation> getObservationUsingId(int id, int? limit = null);


}



