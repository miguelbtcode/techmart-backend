// ✅ Personalización JavaScript para Swagger UI
function runCustomCode() {
  const info = document.querySelector(".swagger-ui .info");
  if (info) {
    const badgeWrapper = document.createElement("div");
    badgeWrapper.style.cssText = `
      margin: 20px 0;
      display: flex;
      gap: 10px;
      flex-wrap: wrap;
      justify-content: flex-start;
    `;

    const badges = [
      { text: "✓ Production Ready", color: "#28a745" },
      { text: "🔒 JWT Auth", color: "#007bff" },
      { text: "📊 Health Checks", color: "#17a2b8" },
      { text: "🚀 .NET 9", color: "#6f42c1" },
    ];

    badges.forEach(({ text, color }) => {
      const span = document.createElement("span");
      span.textContent = text;
      span.style.cssText = `
        background: ${color};
        color: white;
        padding: 4px 12px;
        border-radius: 12px;
        font-size: 0.8rem;
        font-family: 'Segoe UI', sans-serif;
        box-shadow: 0 2px 6px rgba(0,0,0,0.1);
      `;
      badgeWrapper.appendChild(span);
    });

    info.appendChild(badgeWrapper);
  }

  // Auto-colapsar sección "Health"
  const healthSection = document.querySelector('[data-tag="Health"]');
  if (healthSection) {
    const button = healthSection.querySelector(".opblock-tag");
    if (button) button.click();
  }
}

// Esperar a que Swagger termine de renderizar
setTimeout(runCustomCode, 200);
