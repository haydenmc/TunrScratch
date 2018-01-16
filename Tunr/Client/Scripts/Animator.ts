export class Animator {
    public static applyAnimationClass(element: HTMLElement, cssAnimationClassName: string): Promise<void> {
        return new Promise((resolve, reject) => {
            if (element == null) {
                reject("Could not find element to animate.");
            } else {
                let animationEndHandler = (ev: AnimationEvent) => {
                    if (element.classList.contains(cssAnimationClassName)) {
                        element.classList.remove(cssAnimationClassName);
                    }
                    element.removeEventListener("animationend", animationEndHandler, false);
                    resolve();
                };
                element.addEventListener("animationend", animationEndHandler, false);
                if (!element.classList.contains(cssAnimationClassName)) {
                    element.classList.add(cssAnimationClassName);
                }
            }
        });
    }
}
