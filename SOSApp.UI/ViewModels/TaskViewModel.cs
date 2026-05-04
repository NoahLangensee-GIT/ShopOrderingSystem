using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SOSApp.ViewModels;

public class TaskViewModel : INotifyPropertyChanged
{
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    public DateTime? Deadline
    {
        get => _deadline;
        set
        {
            _deadline = value;
            OnPropertyChanged();
        }
    }

    public ICommand DeleteCommand { get; }
    
    public ICommand ModifyNameCommand { get; }
    
    public ICommand ModifyDateCommand { get; }
    
    public ICommand ModifyOrReadDescription { get; }
    
    public ICommand OnCheckedTaskChangedCommand { get; }
    
    private readonly MainWindowViewModel _mainWindowViewModel;
    
    private string _title;
    
    private DateTime? _deadline;
    
    private string _deadlinestring;

    public bool IsCompleted
    {
        get => _iscompleted;
        set
        {
            _iscompleted = value;
            OnPropertyChanged();
        }
    }

    public string DeadlineText
    {
        get => _deadlinestring;
        set
        {
            _deadlinestring = value;
            OnPropertyChanged();
        }
    }
    
    private SolidColorBrush _statusColor;
    private bool _iscompleted;

    public SolidColorBrush StatusColor
    {
        get => _statusColor;
        private set
        {
            _statusColor = value;
            OnPropertyChanged();
        }
    }
    
    public string? Description { get; set; }
    
    public int Id { get; init; }

     public TaskViewModel(string title, DateTime? deadline, string? description, bool iscompleted, MainWindowViewModel mainWindowViewModel)
     {
         
         StatusColor = new SolidColorBrush(Colors.LightBlue);
         _mainWindowViewModel = mainWindowViewModel;
         Description = description;
         Title = title;
         Deadline = deadline;
         IsCompleted = iscompleted;
         DeadlineText = Deadline!.Value.ToShortDateString();
         DeleteCommand = new RelayCommand(DeleteThisTask);
         ModifyNameCommand = new RelayCommand(ModifyNamePopUp);
         ModifyDateCommand = new RelayCommand(ModifyDatePopUp);
         ModifyOrReadDescription = new RelayCommand(ModifyOrReadDescriptionPopUp);
         OnCheckedTaskChangedCommand = new RelayCommand(HandleTaskCompletion);
     }

     private void HandleTaskCompletion()
     {
         _mainWindowViewModel.SetTheCurrentTask(this);
         if (IsCompleted)
         {
             var result = MessageBox.Show(
                 "Are you sure you completed this task?",
                 "Confirmation",
                 MessageBoxButton.YesNo,
                 MessageBoxImage.Question
             );
             if (result == MessageBoxResult.Yes)
             {
                 _mainWindowViewModel.UpdateAllLists();
                 _mainWindowViewModel.HandleSession.HandleCompletion(Id, true);
                 StatusColor = new SolidColorBrush(Colors.LightGreen);   
             }
             else
             {
                 IsCompleted = false;
             }
         }
         else
         {
             _mainWindowViewModel.CompletedTasks = _mainWindowViewModel.CompletedTasks.Where(x => x.IsCompleted).ToList();
             _mainWindowViewModel.CompletedTasks.Remove(this);
             StatusColor = new SolidColorBrush(Colors.LightBlue);
         }
     }

      private void ModifyOrReadDescriptionPopUp()
      {
          _mainWindowViewModel.SetTheCurrentTask(this);
          _mainWindowViewModel.ModifyDescriptionOpen = true;
          _mainWindowViewModel.DescriptionText = Description;
      }

      private void ModifyDatePopUp()
      {
          _mainWindowViewModel.SetTheCurrentTask(this);
          _mainWindowViewModel.ModifyDeadLineOpen = true;
          _mainWindowViewModel.SelectedDate = Deadline;
      }


      private void ModifyNamePopUp()
      {
          _mainWindowViewModel.SetTheCurrentTask(this);
          _mainWindowViewModel.ModifyTitleOpen = true;
          _mainWindowViewModel.ChangeTitleText = Title;
      }

      private void DeleteThisTask()
      { 
          _mainWindowViewModel.SetTheCurrentTask(this);
          _mainWindowViewModel.RemoveASpecificTask(this);
      }
     public event PropertyChangedEventHandler? PropertyChanged;

     protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
     {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
     }
}