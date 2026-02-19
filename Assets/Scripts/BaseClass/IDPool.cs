using System.Collections.Generic;

public class IdPool
{
    Stack<int> freeIds = new Stack<int>();
    int nextId = 0;

    // 새로운 ID 가져오기
    public int GetId()
    {
        // 반납된 ID가 있다면 재사용
        if (freeIds.Count > 0)
        {
            return freeIds.Pop();
        }

        // 없다면 새로운 번호 할당
        return nextId++;
    }

    public void ReturnId(int id)
    {
        freeIds.Push(id);
    }
}