/**
 * Base Component class
 */
export abstract class Component {
    protected componentId: string = null;
    protected element: HTMLElement = null;

    constructor(componentId: string) {
        // Derived components must call the super constructor with the component ID
        this.componentId = componentId;
        if (this.componentId === null || this.componentId === undefined || this.componentId === "") {
            throw "Component ID must be defined";
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
    }

    public insertComponent(parentNode: Node, beforeNode?: Node): void {
        this.element = parentNode.insertBefore(this.element, beforeNode);
    }
}