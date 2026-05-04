using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Contract.Contract;

namespace TODOApp.UI.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{

    private List<TaskViewModel> _allTasks;
    private List<TaskViewModel> _completedtasks;
    public readonly IHandleSessionDataBase HandleSession;
    private List<TaskViewModel> _uncompletedtasks;
    private string? _descriptiontext;
    private DateTime? _selecteddate;
    private string _titletext;
    private bool _titleopen;
    private bool _deadlineopen;
    private bool _descriptionopen;
    private bool _taskopen;
    private string _createtitleinput;
    private DateTime? _createdateinput;
    private string _createdescription;

    public MainWindowViewModel(IHandleSessionDataBase sessionDataBase)
    {
        HandleSession = sessionDataBase;
        AddTaskCommand = new RelayCommand(AddTask);
        CreateTaskCommand = new RelayCommand(TryAndCreateTask);
        ConfirmNameModification = new RelayCommand(TryModifyName);
        ConfirmDateModification = new RelayCommand(TryModifyDate);
        ConfirmDescriptionModification = new RelayCommand(TryModifyDescription);
        CloseTaskCommand = new RelayCommand(() => CreateTaskOpen = false);

        AllTasks = LoadAllTasksFromDatabase();
        UpdateAllLists();
    }

    public ICommand CreateTaskCommand { get; }
    public ICommand CloseTaskCommand { get; }
    public ICommand ConfirmNameModification { get; }
    public ICommand ConfirmDateModification { get; }
    public ICommand ConfirmDescriptionModification { get; }
    public ICommand AddTaskCommand { get; } 
    private TaskViewModel CurrentTask { get; set; }

    public List<TaskViewModel> CompletedTasks
    {
        get => _completedtasks;

        set
        {
            _completedtasks = value;
            OnPropertyChanged();
        }
    }

    public List<TaskViewModel> UncompletedTasks
    {
        get => _uncompletedtasks;

        private set
        {
            _uncompletedtasks = value;
            OnPropertyChanged();
        }
    }

    public bool ModifyDeadLineOpen
    {
        get => _deadlineopen;
        set
        {
            _deadlineopen = value;
            OnPropertyChanged();
        }
    }

    public bool ModifyDescriptionOpen
    {
        get => _descriptionopen;
        set
        {
            _descriptionopen = value;
            OnPropertyChanged();
        }
    }

    public bool ModifyTitleOpen
    {
        get => _titleopen;
        set
        {
            _titleopen = value;
            OnPropertyChanged();
        }
    }

    public bool CreateTaskOpen
    {
        get => _taskopen;
        set
        {
            _taskopen = value;
            OnPropertyChanged();
        }
    }

    public string? DescriptionText
    {
        get => _descriptiontext;
        set
        {
            _descriptiontext = value;
            OnPropertyChanged();
        }
    }

    public DateTime? SelectedDate
    {
        get => _selecteddate;
        set
        {
            _selecteddate = value;
            OnPropertyChanged();
        }
    }

    public string ChangeTitleText
    {
        get => _titletext;
        set
        {
            _titletext = value;
            OnPropertyChanged();
        }
    }
    public string CreateTitleInput
    {
        get => _createtitleinput;
        set
        {
            _createtitleinput = value;
            OnPropertyChanged();
        }
    }

    public DateTime? CreateDateInput
    {
        get => _createdateinput;
        set
        {
            _createdateinput = value;
            OnPropertyChanged();
        }
    }

    public string CreateDescriptionInput
    {
        get => _createdescription;
        set
        {
            _createdescription  = value;
            OnPropertyChanged();
        }
    }
    private List<TaskViewModel> AllTasks
    {
        get => _allTasks;
        set
        {
            _allTasks = value;
            UncompletedTasks = AllTasks.Where(x => !x.IsCompleted).ToList();
            OnPropertyChanged();
        }
    }

    public void SetTheCurrentTask(TaskViewModel task)
    {
        CurrentTask = task;
    }

    public void RemoveASpecificTask(TaskViewModel task)
    {
        HandleSession.DeleteObject(task.Id);
        UpdateAllTasks();
        UpdateAllLists();
    }

    public void UpdateAllLists()
    {
        CompletedTasks = AllTasks.Where(x => x.IsCompleted).ToList();
        UncompletedTasks = AllTasks.Where(x => !x.IsCompleted).ToList();
    }

    private List<TaskViewModel> LoadAllTasksFromDatabase()
    {
        var tasks = new List<TaskViewModel>();
        foreach (var task in HandleSession.GetAllTasks())
        {
            var newtaskviewmodel =
                new TaskViewModel(task.Title, task.Deadline, task.Description, task.IsCompleted, this)
                {
                    Id = task.Id
                };
            tasks.Add(newtaskviewmodel);
        }

        return tasks;
    }

    private void TryModifyDescription()
    {
        CurrentTask.Description = DescriptionText;
        HandleSession.ModifyDescription(CurrentTask.Id, DescriptionText);
        ModifyDescriptionOpen = false;
    }

    private void TryModifyDate()
    {
        if (SelectedDate > DateTime.Today && SelectedDate != null)
        {
            CurrentTask.Deadline = SelectedDate;
            CurrentTask.DeadlineText = SelectedDate.Value.ToShortDateString();
            ModifyDeadLineOpen = false;
            HandleSession.ModifyDate(CurrentTask.Id, SelectedDate);
            AllTasks = AllTasks.OrderByDescending(x => x.Deadline).Reverse().ToList();
        }
        else
        {
            MessageBox.Show("Enter a valid date");
        }
    }

    private void TryModifyName()
    {
        if (ChangeTitleText.Length > 0)
        {
            CurrentTask.Title = ChangeTitleText;
            ModifyTitleOpen = false;
            HandleSession.ModifyName(CurrentTask.Id, ChangeTitleText);
        }
        else
        {
            MessageBox.Show("Input a valid name");
        }
    }

   

    private void TryAndCreateTask()
    {
        if (CreateTitleInput.Length <= 0)
        {
            MessageBox.Show("Please enter a title");
        }

        else if (CreateDateInput < DateTime.Today)
        {
            MessageBox.Show("Please enter a valid date");
        }

        else
        {
            HandleSession.SaveObject(new SaveTaskDto()
            {
                Title = CreateTitleInput,
                Deadline = CreateDateInput,
                Description = CreateDescriptionInput,
            });
            UpdateAllTasks();
            CreateTaskOpen = false;
            CreateTitleInput = string.Empty;
            CreateDateInput = null;
            CreateDescriptionInput = string.Empty;
        }
    }
    
    private void AddTask()
    {
        CreateTaskOpen = true;
    }

    private void UpdateAllTasks() => AllTasks = LoadAllTasksFromDatabase();

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}