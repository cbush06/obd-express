using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObdExpress.Ui.UserControls.Interfaces
{
    public interface IPanelCollection
    {
        List<IRegisteredPanel> Panels { get; }
        void OnPanelCollectionShown();
        void OnPanelCollectionHidden();
    }
}
