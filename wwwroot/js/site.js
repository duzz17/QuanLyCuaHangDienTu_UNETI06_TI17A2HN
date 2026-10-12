(() => {
  "use strict";
  const toggle = document.querySelector("[data-toggle-sidebar]");
  const sidebar = document.getElementById("admin-sidebar");
  let previousFocus = null;
  const closeSidebar = () => {
    document.body.classList.remove("sidebar-open");
    toggle?.setAttribute("aria-expanded", "false");
    previousFocus?.focus();
  };
  toggle?.addEventListener("click", () => {
    const open = document.body.classList.toggle("sidebar-open");
    toggle.setAttribute("aria-expanded", String(open));
    if (open) { previousFocus = document.activeElement; sidebar?.querySelector("a")?.focus(); }
  });
  document.querySelector("[data-close-sidebar]")?.addEventListener("click", closeSidebar);
  document.addEventListener("keydown", event => {
    if (!document.body.classList.contains("sidebar-open")) return;
    if (event.key === "Escape") closeSidebar();
    if (event.key === "Tab" && sidebar) {
      const links = [...sidebar.querySelectorAll("a, button")];
      const first = links[0], last = links[links.length - 1];
      if (event.shiftKey && document.activeElement === first) { event.preventDefault(); last?.focus(); }
      else if (!event.shiftKey && document.activeElement === last) { event.preventDefault(); first?.focus(); }
    }
  });
  window.matchMedia("(min-width: 992px)").addEventListener("change", event => {
    if (event.matches && document.body.classList.contains("sidebar-open")) closeSidebar();
  });
  document.querySelectorAll("[data-submit-on-change]").forEach(select => {
    select.addEventListener("change", () => select.form?.requestSubmit());
  });
  document.querySelectorAll("[data-quantity-step]").forEach(button => {
    button.addEventListener("click", () => {
      const input = button.closest(".quantity-stepper")?.querySelector("input");
      if (!input) return;
      const current = Number.parseInt(input.value, 10) || 1;
      const min = Number.parseInt(input.min, 10) || 1;
      const max = Number.parseInt(input.max, 10) || 2147483647;
      input.value = String(Math.max(min, Math.min(max, current + Number(button.dataset.quantityStep))));
      input.dispatchEvent(new Event("change", { bubbles: true }));
    });
  });
  document.querySelectorAll("[data-toggle-password]").forEach(button => {
    button.addEventListener("click", () => {
      const input = document.getElementById(button.dataset.togglePassword);
      if (!input) return;
      const show = input.type === "password";
      input.type = show ? "text" : "password";
      button.setAttribute("aria-pressed", String(show));
      button.setAttribute("aria-label", show ? "Ẩn mật khẩu" : "Hiện mật khẩu");
    });
  });
  document.querySelectorAll("form[data-confirm]").forEach(form => {
    form.addEventListener("submit", event => { if (!window.confirm(form.dataset.confirm)) event.preventDefault(); });
  });
  const showCartFeedback = (message, warning = false) => {
    const container = document.querySelector("[data-cart-feedback-container]");
    if (!container || !message) return;
    const feedback = document.createElement("div");
    feedback.className = `cart-feedback ${warning ? "warning" : "success"}`;
    feedback.setAttribute("role", warning ? "alert" : "status");
    feedback.textContent = message;
    container.replaceChildren(feedback);
    window.setTimeout(() => feedback.remove(), 3500);
  };
  document.querySelectorAll("form[data-cart-form]").forEach(form => {
    form.addEventListener("submit", async event => {
      const submitter = event.submitter;
      if (!(submitter instanceof HTMLButtonElement) || !submitter.hasAttribute("data-ajax-cart-submit")) return;
      event.preventDefault();
      submitter.disabled = true;
      form.setAttribute("aria-busy", "true");
      try {
        const response = await fetch(form.action, {
          method: "POST",
          body: new FormData(form),
          credentials: "same-origin",
          headers: {
            "Accept": "application/json",
            "X-Requested-With": "XMLHttpRequest"
          }
        });
        if (response.redirected) {
          window.location.assign(response.url);
          return;
        }
        const result = await response.json();
        if (!response.ok || !result.success) throw new Error(result.message || "Không thể thêm sản phẩm vào giỏ hàng.");
        document.querySelectorAll(".cart-count").forEach(count => {
          count.textContent = String(result.cartCount);
        });
        showCartFeedback(result.message, result.warning);
      } catch (error) {
        showCartFeedback(error instanceof Error ? error.message : "Không thể thêm sản phẩm vào giỏ hàng.", true);
      } finally {
        submitter.disabled = false;
        form.removeAttribute("aria-busy");
      }
    });
  });
  document.querySelector("[data-print]")?.addEventListener("click", () => window.print());
  const fallback = document.currentScript?.dataset.productPlaceholder || "/images/product-placeholder.svg";
  document.querySelectorAll("img[data-product-image]").forEach(img => {
    const name = (img.dataset.productName || img.alt || "").toLocaleLowerCase("vi-VN");
    const type = /watch|đồng hồ|band|garmin/.test(name) ? "watch"
      : /laptop|macbook|lenovo|asus|victus|loq|thinkpad/.test(name) ? "laptop"
      : /tai nghe|airpods|buds|headphone/.test(name) ? "headphones"
      : /bàn phím|chuột|logitech|keyboard|mouse/.test(name) ? "keyboard"
      : /điện thoại|iphone|galaxy|xiaomi|oppo|phone/.test(name) ? "phone" : "product";
    const illustration = fallback.replace("product-placeholder.svg", type + "-placeholder.svg");
    const annotate = () => {
      const imageLink = img.closest(".product-image");
      if (imageLink && !imageLink.querySelector(".image-illustration-label")) {
        const note = document.createElement("span");
        note.className = "image-illustration-label";
        note.textContent = "Hình minh họa";
        imageLink.append(note);
      }
      const caption = img.closest(".detail-gallery")?.querySelector(".gallery-caption");
      if (caption) caption.textContent = caption.textContent.replace("Hình ảnh sản phẩm", "Hình minh họa sản phẩm");
    };
    const useFallback = () => {
      if (img.dataset.fallbackApplied) return;
      img.dataset.fallbackApplied = "true";
      img.src = illustration;
      annotate();
    };
    img.addEventListener("error", useFallback);
    if (img.getAttribute("src")?.includes("product-placeholder.svg") || (img.complete && img.naturalWidth === 0)) useFallback();
  });
})();
