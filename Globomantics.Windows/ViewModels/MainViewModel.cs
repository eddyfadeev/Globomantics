﻿using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using Globomantics.Domain;
using Globomantics.Infrastructure.Data.Repositories;
using Globomantics.Windows.Messages;

namespace Globomantics.Windows.ViewModels;

public class MainViewModel : ObservableObject, 
    IViewModel
{
    private string statusText = "Everything is OK!";
    private bool isLoading;
    private bool isInitialized;
    private readonly IRepository<User> userRepository;
    private readonly IRepository<TodoTask> todoRepository;

    public string StatusText 
    {
        get => statusText;
        set
        {
            statusText = value;

            OnPropertyChanged(nameof(StatusText));
        }
    }
    public bool IsLoading
    {
        get => isLoading;
        set
        {
            isLoading = value;

            OnPropertyChanged(nameof(IsLoading));
        }
    }

    public ICommand ExportCommand { get; set; }
    public ICommand ImportCommand { get; set; }

    public Action<string>? ShowAlert { get; set; }
    public Action<string>? ShowError { get; set; }
    public Func<IEnumerable<string>>? ShowOpenFileDialog { get; set; }
    public Func<string>? ShowSaveFileDialog { get; set; }
    public Func<string, bool>? AskForConfirmation { get; set; }
    
    public ObservableCollection<Todo> Completed { get; set; } = new ();
    public ObservableCollection<Todo> Unfinished { get; set; } = new ();
    
    public MainViewModel(IRepository<User> userRepository, IRepository<TodoTask> todoRepository)
    {
        WeakReferenceMessenger.Default.Register<TodoSavedMessage>(this, (sender, message) =>
        {
            var item = message.Value;

            if (item.IsCompleted)
            {
                var existing = Unfinished.FirstOrDefault(i => i.Id == item.Id);

                if (existing is not null)
                {
                    Unfinished.Remove(existing);
                }

                ReplaceOrAdd(Completed, item);
            }
            else
            {
                var existing = Completed.FirstOrDefault(i => i.Id == item.Id);

                if (existing is not null)
                {
                    Completed.Remove(existing);
                }
                
                ReplaceOrAdd(Unfinished, item);
            }
        });
        
        WeakReferenceMessenger.Default.Register<TodoDeletedMessage>(this, (sender, message) =>
        {
            var item = message.Value;
            
            var unfinishedItem = Unfinished.FirstOrDefault(i => i.Id == item.Id);
            
            if (unfinishedItem is not null)
            {
                Unfinished.Remove(unfinishedItem);
            }
        });
        
        this.userRepository = userRepository;
        this.todoRepository = todoRepository;
    }

    private void ReplaceOrAdd(ObservableCollection<Todo> collection, Todo item)
    {
        var existingItem = collection.FirstOrDefault(x => x.Id == item.Id);

        if (existingItem is not null)
        {
            var index = Unfinished.IndexOf(existingItem);
            collection[index] = item;
        }
        else
        {
            collection.Add(item);
        }
    }

    public async Task InitializeAsync()
    {
        if (isInitialized)
        {
            return;
        }
        
        App.CurrentUser = await userRepository.FindByAsync("Eddy");

        var items = await todoRepository.AllAsync();
        
        var itemsDue = items.Count(i => i.DueDate.ToLocalTime() > DateTimeOffset.Now);
        
        StatusText = $"Welcome {App.CurrentUser.Name}! You have {itemsDue} items passes due date.";

        foreach (var item in items.Where(item => !item.IsDeleted))
        {
            if (item.IsCompleted)
            {
                Completed.Add(item);
            }
            else
            {
                Unfinished.Add(item);
            }
        }

        isInitialized = true;
    }
}