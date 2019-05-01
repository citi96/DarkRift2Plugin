using System.ComponentModel;

namespace Domain.Domain {
    public interface IProcessDirty : INotifyPropertyChanged {
        bool IsNew { get; }
        bool IsDirty { get; }
        bool IsDeleted { get; }
    }
}
