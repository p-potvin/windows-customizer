import * as React from "react";
const SVGComponent = (props) => (
  <svg
    width={20}
    height={20}
    viewBox="0 0 24 24"
    fill="none"
    stroke="#21B8CC"
    strokeWidth={1.75}
    {...props}
  >
    <path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10z" />
  </svg>
);
export default SVGComponent;
