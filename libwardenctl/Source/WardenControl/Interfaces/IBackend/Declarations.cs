namespace WardenControl;

public interface IBackend {
    Status Create      (Configuration Configuration);
    Status Reconfigure (Configuration Configuration);
    Status Destroy     (Configuration Configuration);
    
    Status Start       (Configuration Configuration);
    Status Stop        (Configuration Configuration);
    Status Lock        (Configuration Configuration);
    Status Unlock      (Configuration Configuration);
    Status Enable      (Configuration Configuration);
    Status Disable     (Configuration Configuration);
    
    Status Query       (Configuration Configuration, out Report Report);
    Status Observe     (Configuration Configuration, out StreamReader LogStream);
}