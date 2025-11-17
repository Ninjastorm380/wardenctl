namespace WardenControl.Containers;

public static class Actions {
    // "systemd-nspawn", $"-qbPD {MountPath} -M {UID} --hostname {Hostname} --network-macvlan={Interface} --setenv=SYSTEMD_JOURNAL_STORAGE=volatile --bind=/dev/null:/etc/systemd/system/systemd-journal-flush.service"

    public static Int32 Start(Container Container, ResourceConfiguration Configuration, out Metrics Metrics) {
        
    }
}