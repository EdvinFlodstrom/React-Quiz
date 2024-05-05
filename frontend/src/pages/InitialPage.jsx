import React, { useState } from "react";
import Instructions from "../components/Instructions";
import InitializeQuiz from "../components/InitializeQuiz";
import "../styles/buttons.css"

const InitialPage = () => {
    const [componentToShow, setComponentToShow] = useState(null);

    return (
        <>
            {!componentToShow ? (
                <>
                    <Instructions />
                    <button className="optionsButton" onClick={() => setComponentToShow(<InitializeQuiz />)}>Take Quiz</button>
                </>
            ) : (
                componentToShow
            )}
        </>
    );
};

export default InitialPage;