using Microsoft.AspNetCore.Components;

namespace BlazorRtsPlayer.BlazorApp.Pages
{
    partial class Index: IDisposable
    {
        private bool disposedValue;

        [Inject]
        public RtspInMemoryService rtspInMemoryService { get; set; }

       
        public string Bitmap { get; set; }

        public CancellationTokenSource CancellationTokenSource { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            rtspInMemoryService.Snapshot += RtspInMemoryService_Snapshot;
            await rtspInMemoryService.ExecuteAsync();
        }

        private async void RtspInMemoryService_Snapshot(string bitmap)
        {

            await InvokeAsync(() =>
            {
                Bitmap = $"data:image/jpg;base64,{bitmap}";
                StateHasChanged();
                
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Index()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
