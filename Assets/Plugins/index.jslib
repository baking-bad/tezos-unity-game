mergeInto(LibraryManager.library, {
  ShowCaptchaJS: function () {
    showCaptcha();
  },
  CopyToClipboardJS: function (str) {
    var textarea = document.createElement("textarea");
    textarea.value = UTF8ToString(str);
    document.body.appendChild(textarea);
    textarea.select();
    document.execCommand("copy");
    document.body.removeChild(textarea);
  }
});
