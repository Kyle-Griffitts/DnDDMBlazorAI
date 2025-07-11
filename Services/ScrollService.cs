using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

    public class ScrollService
    {
        private readonly IJSRuntime _js;

        public ScrollService(IJSRuntime js)
        {
            _js = js;
        }

    public async Task ScrollToBottomAsync(ElementReference element)
    {
        await _js.InvokeVoidAsync("scrollToBottom", element);
    }
}

