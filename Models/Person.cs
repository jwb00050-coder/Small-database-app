using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmallDatabaseApp.Models;

public class Person : INotifyPropertyChanged
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private int _age;

    public int Id { get; set; }

    public string FirstName
    {
        get => _firstName;
        set => SetField(ref _firstName, value);
    }

    public string LastName
    {
        get => _lastName;
        set => SetField(ref _lastName, value);
    }

    public int Age
    {
        get => _age;
        set => SetField(ref _age, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
