using System;

interface IButtonAction
{
    public event Action<int,int, BaitButtonAction> BaitChangeAction;
}