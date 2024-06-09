using System;

public interface IButtonAction
{
    public event Action<int,int> BaitChangeAction;
}