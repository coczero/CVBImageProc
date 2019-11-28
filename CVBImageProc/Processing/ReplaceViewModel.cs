﻿using CVBImageProc.Processing.PixelFilter;
using System;

namespace CVBImageProc.Processing
{
  /// <summary>
  /// ViewModel for the <see cref="Replace"/> processor.
  /// </summary>
  class ReplaceViewModel : PlaneProcessorViewModelBase
  {
    #region Properties

    /// <summary>
    /// Value to use when replacing.
    /// </summary>
    public byte ReplaceWith
    {
      get => _processor.ReplaceWith;
      set
      {
        if (ReplaceWith != value)
        {
          _processor.ReplaceWith = value;
          NotifyOfPropertyChange();
          OnSettingsChanged();
        }
      }
    }

    /// <summary>
    /// ViewModel for the processors pixel filter chain.
    /// </summary>
    public PixelFilterChainViewModel PixelFilterChainVM { get; }

    #endregion Properties

    #region Member

    /// <summary>
    /// The processor.
    /// </summary>
    private readonly Replace _processor;

    #endregion Member

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="processor">The processor.</param>
    public ReplaceViewModel(Replace processor)
      : base(processor)
    {
      _processor = processor;
      PixelFilterChainVM = new PixelFilterChainViewModel(_processor.PixelFilter);
      PixelFilterChainVM.SettingsChanged += PixelFilterChainVM_SettingsChanged;
    }

    /// <summary>
    /// Fires the SettingsChanged event when the
    /// pixel filter settings changed.
    /// </summary>
    /// <param name="sender">Ignored.</param>
    /// <param name="e">Ignored.</param>
    private void PixelFilterChainVM_SettingsChanged(object sender, EventArgs e)
    {
      OnSettingsChanged();
    }
  }
}