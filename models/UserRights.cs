using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpisCentralDisplayController.models
{
    public enum UserRights
    {
        WorkspaceCreate,
        WorkspaceUpdate,
        WorkspaceDelete,
        UserCategoriesCreate,
        UserCategoriesRead,
        UserCategoriesUpdate,
        UserCategoriesDelete,
        UserCreate,
        UserRead,
        UserUpdate,
        UserDelete,
        StationInfoCreate,
        StationInfoRead,
        StationInfoUpdate,
        DisplaysCreate,
        DisplaysRead,
        DisplaysUpdate,
        DisplaysDelete,
        NetworkConfigurationCreate,
        NetworkConfigurationRead,
        NetworkConfigurationUpdate,
        NetworkConfigurationDelete,
        TrainInfoCreate,
        TrainInfoRead,
        TrainInfoUpdate,
        TrainInfoDelete,
        SoundsRecord,
        SoundsPlay,
        SoundsDelete,
        PlaylistCreate,
        PlaylistRead,
        PlaylistUpdate,
        PlaylistDelete,
        RMSServerSettingsCreate,
        RMSServerSettingsRead,
        RMSServerSettingsUpdate,
        RMSServerOperate,
        MediaCreate,
        MediaRead,
        MediaUpdate,
        MediaDelete,
        ReportsCreate,
        ReportsRead,
        ReportsDelete,
        BackupCreate,
        BackupDelete,
        SystemRestoreOperate
    }
}
