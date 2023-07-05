export default window.__litium.constants;

export const ViewMode = {
    List: 'list',
    Edit: 'edit',
    Detail: 'detail',
};

export const PaginationOptions = {
    PageSize: 5, // Number of items per page
    DisplayedEntries: 4, // Specifies how many links to show excluding possible EdgeEntries
    EdgeEntries: 2, // Specifies how many links from beginning and end to show ex. 1 2 3 ... 10 20 30 ... 97 98 99 where "1 2 3" and "97 98 99" are edge entries
};

export const ShippingIntegrationType = {
    Inline: 'Inline',
    DeliveryCheckout: 'DeliveryCheckout',
    PaymentCheckout: 'PaymentCheckout',
};

export const PaymentIntegrationType = {
    IframeCheckout: 'IframeCheckout',
    PaymentWidgets: 'PaymentWidgets',
    DirectPayment: 'DirectPayment',
};

export const ADDRESS_RECEIVE = 'ADDRESS_RECEIVE';
export const ADDRESS_ERROR = 'ADDRESS_ERROR';
export const ADDRESS_CHANGE_MODE = 'ADDRESS_CHANGE_MODE';

export const CART_LOAD_ERROR = 'CART_LOAD_ERROR';
export const CART_RECEIVE = 'CART_RECEIVE';
export const CART_SHOW_INFO = 'CART_SHOW_INFO';

export const CHECKOUT_SET_SELECTED_COMPANY_ADDRESS =
    'CHECKOUT_SET_SELECTED_COMPANY_ADDRESS';
export const CHECKOUT_SET_PRIVATE_CUSTOMER = 'CHECKOUT_SET_PRIVATE_CUSTOMER';
export const CHECKOUT_SET_SIGN_UP = 'CHECKOUT_SET_SIGN_UP';
export const CHECKOUT_SET_DELIVERY = 'CHECKOUT_SET_DELIVERY';
export const CHECKOUT_SET_COUNTRY = 'CHECKOUT_SET_COUNTRY';
export const CHECKOUT_SET_PAYMENT = 'CHECKOUT_SET_PAYMENT';
export const CHECKOUT_SET_DISCOUNT_CODE = 'CHECKOUT_SET_DISCOUNT_CODE';
export const CHECKOUT_SET_USED_DISCOUNT_CODE =
    'CHECKOUT_SET_UASED_DISCOUNT_CODE';
export const CHECKOUT_SET_ORDER_NOTE = 'CHECKOUT_SET_ORDER_NOTE';
export const CHECKOUT_ACCEPT_TERMS_OF_CONDITION =
    'CHECKOUT_ACCEPT_TERMS_OF_CONDITION';
export const CHECKOUT_SUBMIT = 'CHECKOUT_SUBMIT';
export const CHECKOUT_SUBMIT_ERROR = 'CHECKOUT_SUBMIT_ERROR';
export const CHECKOUT_SET_PAYMENT_WIDGET = 'CHECKOUT_SET_PAYMENT_WIDGET';
export const CHECKOUT_UPDATE_CUSTOMER_INFO = 'CHECKOUT_UPDATE_CUSTOMER_INFO';
export const CHECKOUT_SET_SHOW_ALTERNATIVE_ADDRESS =
    'CHECKOUT_SET_SHOW_ALTERNATIVE_ADDRESS';
export const CHECKOUT_SET_STATUS_SUBMIT_BUTTON =
    'CHECKOUT_SET_STATUS_SUBMIT_BUTTON';
export const CLEAR_ERROR = 'CLEAR_ERROR';
export const CHECKOUT_VALIDATE_ADDRESS = 'CHECKOUT_VALIDATE_ADDRESS';

export const FACETED_SEARCH_QUERY = 'FACETED_SEARCH_QUERY';
export const FACETED_SEARCH_RECEIVE = 'FACETED_SEARCH_RECEIVE';
export const FACETED_SEARCH_ERROR = 'FACETED_SEARCH_ERROR';
export const FACETED_SEARCH_TOGGLE_VALUE = 'FACETED_SEARCH_TOGGLE_VALUE';
export const FACETED_SEARCH_TOGGLE_COMPACT = 'FACETED_SEARCH_TOGGLE_COMPACT';
export const PRODUCT_VIEW_CACHED = 'PRODUCT_VIEW_CACHED';

export const LIGHTBOX_IMAGES_SET_CURRENT_IMAGE =
    'LIGHTBOX_IMAGES_SET_CURRENT_IMAGE';
export const LIGHTBOX_IMAGES_SHOW = 'LIGHTBOX_IMAGES_SHOW';

export const NAVIGATION_LOAD_ERROR = 'NAVIGATION_LOAD_ERROR';
export const NAVIGATION_RECEIVE = 'NAVIGATION_RECEIVE';

export const PERSON_RECEIVE = 'PERSON_RECEIVE';
export const PERSON_ERROR = 'PERSON_ERROR';
export const PERSON_CHANGE_MODE = 'PERSON_CHANGE_MODE';

export const QUICK_SEARCH_QUERY = 'QUICK_SEARCH_QUERY';
export const QUICK_SEARCH_RECEIVE = 'QUICK_SEARCH_RECEIVE';
export const QUICK_SEARCH_ERROR = 'QUICK_SEARCH_ERROR';
export const QUICK_SEARCH_SHOW_FULL_FORM = 'QUICK_SEARCH_SHOW_FULL_FORM';
export const QUICK_SEARCH_SELECT_ITEM = 'QUICK_SEARCH_SELECT_ITEM';

export const ORDER_RECEIVE = 'ORDER_RECEIVE';
export const ORDER_ERROR = 'ORDER_ERROR';
export const ORDER_CHANGE_MODE = 'ORDER_CHANGE_MODE';
export const ORDER_CHANGE_CURRENTPAGE = 'ORDER_CHANGE_CURRENTPAGE';
export const ORDER_SET_ORDER = 'ORDER_SET_ORDER';