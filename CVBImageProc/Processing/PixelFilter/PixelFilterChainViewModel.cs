﻿using CVBImageProc.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace CVBImageProc.Processing.PixelFilter
{
  /// <summary>
  /// ViewModel for a <see cref="PixelFilterChain"/>.
  /// </summary>
  class PixelFilterChainViewModel : ViewModelBase, IHasSettings
  {
    #region IHasSettings Implementation

    /// <summary>
    /// Event that is fired when one of
    /// the settings changed.
    /// </summary>
    public event EventHandler SettingsChanged;

    #endregion IHasSettings Implementation

    #region Commands

    /// <summary>
    /// Command for adding a new pixel filter.
    /// </summary>
    public ICommand AddPixelFilterCommand { get; }

    /// <summary>
    /// Command for removing the selected pixel filter.
    /// </summary>
    public ICommand RemoveSelectedPixelFilterCommand { get; }

    #endregion Commands

    #region Properties

    /// <summary>
    /// The configured filters.
    /// </summary>
    public ObservableCollection<IPixelFilterViewModel> Filters { get; }

    /// <summary>
    /// Currently selected filter.
    /// </summary>
    public IPixelFilterViewModel SelectedFilter
    {
      get => _selectedFilter;
      set
      {
        if (SelectedFilter != value)
        {
          _selectedFilter = value;
          NotifyOfPropertyChange();
        }
      }
    }
    private IPixelFilterViewModel _selectedFilter;

    /// <summary>
    /// List of available filter types.
    /// </summary>
    public IEnumerable<TypeViewModel> AvailableFilter { get; }

    /// <summary>
    /// Currently selected filter type.
    /// </summary>
    public TypeViewModel SelectedFilterType
    {
      get => _selectedFilterType;
      set
      {
        if (SelectedFilterType != value)
        {
          _selectedFilterType = value;
          NotifyOfPropertyChange();
        }
      }
    }
    private TypeViewModel _selectedFilterType;

    #endregion Properties

    #region Member

    /// <summary>
    /// The filter chain.
    /// </summary>
    private readonly PixelFilterChain _filterChain;

    #endregion Member

    #region Construction

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="filterChain">The filter chain.</param>
    public PixelFilterChainViewModel(PixelFilterChain filterChain)
    {
      _filterChain = filterChain ?? throw new ArgumentNullException(nameof(filterChain));
      AddPixelFilterCommand = new DelegateCommand((o) => AddPixelFilter());
      RemoveSelectedPixelFilterCommand = new DelegateCommand((o) => RemoveSelectedPixelFilter());

      AvailableFilter = Assembly.GetExecutingAssembly().GetTypes()
           .Where(mytype => mytype.GetInterfaces().Contains(typeof(IPixelFilter)) && !mytype.IsAbstract).Select(i => new TypeViewModel(i)).ToArray();
      SelectedFilterType = AvailableFilter.FirstOrDefault();

      Filters = new ObservableCollection<IPixelFilterViewModel>();
      Filters.CollectionChanged += Filters_CollectionChanged;

      foreach (var filter in _filterChain.Filters)
        Filters.Add(CreatePixelFilterViewModel(filter));
    }

    #endregion Construction

    /// <summary>
    /// Adds a new pixel filter.
    /// </summary>
    private void AddPixelFilter()
    {
      if (SelectedFilterType == null)
        return;

      // add to model
      _filterChain.Filters.Add((IPixelFilter)SelectedFilterType.Instanciate());

      // add to vm
      Filters.Add(CreatePixelFilterViewModel(_filterChain.Filters.Last()));
      SelectedFilter = Filters.Last();
    }

    /// <summary>
    /// Creates a ViewModel for the given <paramref name="filter"/>.
    /// </summary>
    /// <param name="filter">Filter to create ViewModel for.</param>
    /// <returns>ViewModel for the <paramref name="filter"/>.</returns>
    private IPixelFilterViewModel CreatePixelFilterViewModel(IPixelFilter filter)
    {
      if (filter == null)
        throw new ArgumentNullException(nameof(filter));

      switch (filter)
      {
        default:
          return new PixelFilterViewModel(filter);
      }
    }

    /// <summary>
    /// Removes the <see cref="SelectedFilter"/>.
    /// </summary>
    private void RemoveSelectedPixelFilter()
    {
      if (SelectedFilter == null)
        return;

      // remove from model
      int index = Filters.IndexOf(SelectedFilter);
      _filterChain.Filters.RemoveAt(index);

      // remove from vm
      Filters.RemoveAt(index);
    }

    /// <summary>
    /// Links / unlinks events when the <see cref="Filters"/> changed.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Contains the event data.</param>
    private void Filters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        foreach (var filter in e.NewItems.OfType<IHasSettings>())
          filter.SettingsChanged += Filter_SettingsChanged;
      }
      else if (e.Action == NotifyCollectionChangedAction.Remove)
      {
        foreach (var filter in e.OldItems.OfType<IHasSettings>())
          filter.SettingsChanged -= Filter_SettingsChanged;
      }

      SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Fires the <see cref="SettingsChanged"/> event
    /// when the settings of a filter changed.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void Filter_SettingsChanged(object sender, EventArgs e)
    {
      SettingsChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}