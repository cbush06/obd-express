using System.Collections.Generic;

namespace ObdExpress.Ui.UserControls.Interfaces
{
    public interface IPanelCollection
    {
        List<IRegisteredPanel> Panels { get; }
        void OnPanelCollectionShown();
        void OnPanelCollectionHidden();
    }
}
