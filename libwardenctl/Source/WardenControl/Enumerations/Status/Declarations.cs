namespace WardenControl;

[Flags] public enum Status {
    Success                           = 000,
    NotEnoughStorageCapacity          = 001,
    NotEnoughStorageWriteBandwidth    = 002,
    NotEnoughStorageReadBandwidth     = 004,
    NotEnoughStorageWriteIOPS         = 008,
    NotEnoughStorageReadIOPS          = 016,
    NotEnoughMemoryCapacity           = 032,
    NotEnoughLogicalCores             = 064,
    NotEnoughNetworkUploadBandwidth   = 128,
    NotEnoughNetworkDownloadBandwidth = 256,
    DoesNotExist                      = 512
}