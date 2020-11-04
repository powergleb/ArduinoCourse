module.exports = {
    Lesson:
    {
        Theory: [
            {
                Title: "info1",
                Text: "test_text_1",
                Pic: []
            },
            {
                Title: "info2",
                Text: "test_text_2",
                Pic: ["./lessons/test_lesson/pic/pic1.jpg"]
            },
        ],
        Tests: [
            {
                Title: "test1",
                Pic: [],
                Text: "test_test_1",
                Variants: [
                    "False1",
                    "False2",
                    "True",
                    "False3",                    
                ],
                Answer: 2
            },
            {
                Title: "test2",
                Pic: ["./lessons/test_lesson/pic/pic1.jpg"],
                Text: "test_test_2",
                Variants: [
                    "False1",
                    "False2",
                    "True",
                    "False3",                    
                ],
                Answer: 2
            },
        ]
    }
}