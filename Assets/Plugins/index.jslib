mergeInto(LibraryManager.library, {
  ShowCaptchaJS: function () {
    showCaptcha();
  },
  CopyToClipboardJS: function (str) {
    navigator.clipboard.writeText(UTF8ToString(str));
  }
});
