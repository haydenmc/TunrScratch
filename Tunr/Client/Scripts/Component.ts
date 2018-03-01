import { Tunr } from "./Tunr";

/**
 * Base Component class
 */
export abstract class Component {
    protected tunrInstance: Tunr = null;
    protected componentId: string = null;
    protected element: HTMLElement = null;

    constructor(componentId: string, tunrInstance?: Tunr) {
        // Derived components must call the super constructor with the component ID
        this.componentId = componentId;
        if (this.componentId === null || this.componentId === undefined || this.componentId === "") {
            throw "Component ID must be defined";
        }

        // Components maintain a reference to the instance of Tunr they are part of
        if (tunrInstance) {
            this.tunrInstance = tunrInstance;
        } else if (this.tunrInstance === null)
        {
            throw "Component must maintain a reference to Tunr instance";
        }

        // Clone the template for this component in memory
        let template = <HTMLTemplateElement> document.querySelector("template#" + componentId);
        if (template === null) {
            throw "No template element found for component";
        }
        if (template.content.firstElementChild === null) {
            throw "Component template does not contain a root element";
        }
        this.element = <HTMLElement> template.content.firstElementChild.cloneNode(true);

        // Set styles
        if (!this.element.classList.contains("component")) {
            this.element.classList.add("component");
        }
        let cssComponentId = componentId.charAt(0).toLowerCase() + componentId.substr(1);
        if (!this.element.classList.contains(cssComponentId)) {
            this.element.classList.add(cssComponentId);
        }
    }

    public insertComponent(parentNode: Node, beforeNode?: Node): void {
        this.element = parentNode.insertBefore(this.element, beforeNode);
    }

    public removeComponent(): void {
        this.element = this.element.parentNode.removeChild(this.element);
    }
}
