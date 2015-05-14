#region Usings

using System;
using System.Collections.Generic;
using Logictracker.Types.SecurityObjects;
using Logictracker.Web.Controls;
using Logictracker.Web.CustomWebControls.Binding;

#endregion

namespace Logictracker.Web.CustomWebControls.BaseControls.CommonInterfaces
{
    /// <summary>
    /// It defines the way a Group Would look like if UseOptionGroup is true.
    /// </summary>
    public class OptionGroupConfiguration
    {
        public string OptionGroupDescription { get; set; }
        public string ImageUrl { get; set; }
        public string BackgroundColor { get; set; }
        public string Color { get; set; }
    }

    /// <summary>
    /// Defines a common interface for auto bindeable controls.
    /// </summary>
    public interface IAutoBindeable : IAutoBindeableBase
    {
        #region Properties

        /// <summary>
        /// Associated data T of the auto bindeable control.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Determines if the control forces or not postback when its selected index change.
        /// </summary>
        bool AutoPostBack { get; set; }

        /// <summary>
        /// Gets the first selected value.
        /// </summary>
        int Selected { get; }

        /// <summary>
        /// Gets the list of selected values.
        /// </summary>
        List<int> SelectedValues { get; }

        /// <summary>
        /// Gets the logged in user.
        /// </summary>
        UserSessionData Usuario { get; }

        /// <summary>
        /// Determines if the "all item" should be binded.
        /// </summary>
        bool AddAllItem { get; }

        /// <summary>
        /// Determines if the "none item" should be binded.
        /// </summary>
        bool AddNoneItem { get; }

        /// <summary>
        /// Determines if the option group should be considered on data binding.
        /// </summary>
        bool UseOptionGroup { get; }

        /// <summary>
        /// Determines if data assigned to all parents should be considered on data binding.
        /// </summary>
        bool FilterMode { get; }

        /// <summary>
        /// Class for handling data bindings.
        /// </summary>
        BindingManager BindingManager { get; }

        /// <summary>
        /// Determines if the current control remembers its selected values in session.
        /// </summary>
        bool RememberSessionValues { get; }

        int AllValue { get; }
        int NoneValue { get; }

        string AllItemsName { get; }
        string NoneItemsName { get; }

        #endregion

        #region Public Events

        /// <summary>
        /// Event that handles the change of the selected index.
        /// </summary>
        event EventHandler SelectedIndexChanged;

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the parent controls of the indicated T.
        /// </summary>
        /// <param name="type">The T associated to the control.</param>
        /// <returns>The parent control if exist or null.</returns>
        IAutoBindeable GetParent<T>();

        int ParentSelected<T>();
        List<int> ParentSelectedValues<T>();

        string FilterText { get; }

        /// <summary>
        /// Clear all items of the control.
        /// </summary>
        void ClearItems();

        /// <summary>
        /// Adds a new item with the specified value.
        /// </summary>
        /// <param name="text">The text of the item.</param>
        /// <param name="value">The associated value.</param>
        void AddItem(string text, int value);

        void AddItem(string text, string value);

        /// <summary>
        /// Adds item with option group.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <param name="group"></param>
        void AddItem(string text, int value, OptionGroupConfiguration group);

        /// <summary>
        /// Adds a new item at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text"></param>
        /// <param name="value"></param>
        void AddItemAt(Int32 index, String text, Int32 value);

        /// <summary>
        /// Adds a new item at the specified index with the givenn option group.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <param name="group"></param>
        void AddItemAt(Int32 index, String text, Int32 value, OptionGroupConfiguration group);

        /// <summary>
        /// Removes the item associated to the specified value.
        /// </summary>
        /// <param name="value"></param>
        void RemoveItem(int value);

        #endregion
    }
}