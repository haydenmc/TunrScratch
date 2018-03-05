import { Tunr } from "./Tunr";
import { DataModel } from "./Data/DataModel";

/**
 * Base Component class
 */
export abstract class Component {
    protected componentId: string = null;
    protected element: HTMLElement = null;
    protected tunrInstance: Tunr = null;
    protected get dataModel(): DataModel {
        return this.tunrInstance.dataModel;
    }

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

        // Set styles
        if (!this.element.classList.contains("component")) {
            this.element.classList.add("component");
        }
        let cssComponentId = componentId.charAt(0).toLowerCase() + componentId.substr(1);
        if (!this.element.classList.contains(cssComponentId)) {
            this.element.classList.add(cssComponentId);
        }
    }

    public initialize(): void {
        // Meant to provide a place for the component to set-up after being instantiated.
    }

    public static createComponent<T extends Component>(componentType: { new(...args: any[]): T; }, tunrInstance: Tunr, ...args: any[]): T {
        var newComponent = new componentType(...args);
        newComponent.tunrInstance = tunrInstance;
        newComponent.initialize();
        return newComponent;
    }

    /**
     * Used to populate references to common component services
     * @param componentType 
     * @param args 
     */
    protected createComponent<T extends Component>(componentType: { new(...args: any[]): T; }, ...args: any[]): T {
        var newComponent = new componentType(...args);
        newComponent.tunrInstance = this.tunrInstance;
        newComponent.initialize();
        return newComponent;
    }

    public insertComponent(parentNode: Node, beforeNode?: Node): void {
        this.element = parentNode.insertBefore(this.element, beforeNode);
    }

    public removeComponent(): void {
        this.element = this.element.parentNode.removeChild(this.element);
    }
}
