using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using SmallDatabaseApp.Data;
using SmallDatabaseApp.Models;

namespace SmallDatabaseApp;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private readonly AppDbContext _dbContext = new();
    private List<Person> _allPeople = [];
    private Person? _selectedPerson;
    private string _searchText = string.Empty;
    private SortMode _sortMode = SortMode.Name;

    public ObservableCollection<Person> People { get; } = [];

    public Person? SelectedPerson
    {
        get => _selectedPerson;
        set
        {
            if (ReferenceEquals(_selectedPerson, value))
            {
                return;
            }

            _selectedPerson = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        LoadPeople();
    }

    private void LoadPeople()
    {
        _dbContext.Database.EnsureCreated();
        _allPeople = _dbContext.People.ToList();

        if (_allPeople.Count == 0)
        {
            _allPeople.AddRange(
            [
                new Person { FirstName = "Ada", LastName = "Lovelace", Age = 36 },
                new Person { FirstName = "Alan", LastName = "Turing", Age = 41 },
                new Person { FirstName = "Grace", LastName = "Hopper", Age = 85 }
            ]);
            _dbContext.People.AddRange(_allPeople);
            _dbContext.SaveChanges();
        }

        RefreshPeople();
        SelectedPerson = People.FirstOrDefault();
    }

    private void RefreshPeople()
    {
        IEnumerable<Person> filteredPeople = _allPeople;
        var search = _searchText.Trim();

        if (search.Length > 0)
        {
            filteredPeople = filteredPeople.Where(person =>
                person.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                person.LastName.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        filteredPeople = _sortMode == SortMode.Age
            ? filteredPeople.OrderBy(person => person.Age).ThenBy(person => person.LastName)
            : filteredPeople.OrderBy(person => person.LastName).ThenBy(person => person.FirstName);

        People.Clear();
        foreach (var person in filteredPeople)
        {
            People.Add(person);
        }

        if (SelectedPerson is not null && !People.Contains(SelectedPerson))
        {
            SelectedPerson = People.FirstOrDefault();
        }
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _searchText = SearchTextBox.Text;
        RefreshPeople();
    }

    private void SortByNameButton_Click(object sender, RoutedEventArgs e)
    {
        _sortMode = SortMode.Name;
        SortStatusText.Text = "Sorted by last name, then first name";
        RefreshPeople();
    }

    private void SortByAgeButton_Click(object sender, RoutedEventArgs e)
    {
        _sortMode = SortMode.Age;
        SortStatusText.Text = "Sorted by age";
        RefreshPeople();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var newPerson = new Person { FirstName = "New", LastName = "Person", Age = 0 };
        _dbContext.People.Add(newPerson);
        _allPeople.Add(newPerson);
        RefreshPeople();
        SelectedPerson = newPerson;
        PeopleDataGrid.SelectedItem = newPerson;
        PeopleDataGrid.ScrollIntoView(newPerson);
        FirstNameTextBox.Focus();
        FirstNameTextBox.SelectAll();
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (SelectedPerson is null)
        {
            return;
        }

        var personToDelete = SelectedPerson;
        _dbContext.People.Remove(personToDelete);
        _allPeople.Remove(personToDelete);
        RefreshPeople();
        SelectedPerson = People.FirstOrDefault();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        _dbContext.SaveChanges();
        MessageBox.Show("Changes saved.", "People Database", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    protected override void OnClosed(EventArgs e)
    {
        _dbContext.Dispose();
        base.OnClosed(e);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private enum SortMode
    {
        Name,
        Age
    }
}
